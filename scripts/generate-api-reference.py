#!/usr/bin/env python3
from __future__ import annotations

import datetime
import re
from collections import defaultdict
from pathlib import Path
from typing import Dict, List, Optional, Tuple
import xml.etree.ElementTree as ET


ROOT = Path(__file__).resolve().parents[1]
SRC_ROOT = ROOT / "src"
DOCS_PATH = ROOT / "docs" / "API_REFERENCE.md"


TYPE_ALIASES = {
    "System.String": "string",
    "System.Char": "char",
    "System.Boolean": "bool",
    "System.Byte": "byte",
    "System.SByte": "sbyte",
    "System.Int16": "short",
    "System.UInt16": "ushort",
    "System.Int32": "int",
    "System.UInt32": "uint",
    "System.Int64": "long",
    "System.UInt64": "ulong",
    "System.Single": "float",
    "System.Double": "double",
    "System.Decimal": "decimal",
    "System.Object": "object",
    "System.Void": "void",
}


def iter_files(root: Path, pattern: str) -> List[Path]:
    return [
        path
        for path in root.rglob(pattern)
        if "obj" not in path.parts and "bin" not in path.parts
    ]


def extract_namespace(text: str, pattern: re.Pattern) -> Optional[str]:
    match = pattern.search(text)
    return match.group(1) if match else None


def extract_static_classes(text: str, pattern: re.Pattern) -> List[str]:
    return pattern.findall(text)


def get_static_type_map() -> Dict[str, bool]:
    static_types: Dict[str, bool] = {}
    namespace_pattern = re.compile(r"^\s*namespace\s+([\w.]+)\s*[;{]", re.MULTILINE)
    static_class_pattern = re.compile(
        r"\bstatic\s+(?:partial\s+)?class\s+([A-Za-z_]\w*)\b"
    )

    for path in iter_files(SRC_ROOT, "*.cs"):
        try:
            text = path.read_text(encoding="utf-8", errors="ignore")
        except OSError:
            continue

        namespace = extract_namespace(text, namespace_pattern)
        for class_name in extract_static_classes(text, static_class_pattern):
            if namespace:
                static_types[f"{namespace}.{class_name}"] = True
            else:
                static_types[class_name] = True

    return static_types


def count_params_in_signature(param_text: str) -> int:
    if not param_text.strip():
        return 0
    depth = 0
    count = 1
    for char in param_text:
        if char in "<([{":
            depth += 1
        elif char in ">)]}":
            depth -= 1
        elif char == "," and depth == 0:
            count += 1
    return count


def find_matching_paren(text: str, start_index: int) -> int:
    depth = 0
    for index in range(start_index, len(text)):
        char = text[index]
        if char == "(":
            depth += 1
        elif char == ")":
            depth -= 1
            if depth == 0:
                return index
    return -1


def is_method_candidate(line: str) -> bool:
    # Match both block-bodied and expression-bodied methods
    has_static = "static" in line
    has_paren = "(" in line
    has_semicolon = ";" in line
    has_arrow = "=>" in line

    # Block-bodied: static ... (...) { }  (no semicolon on declaration line)
    # Expression-bodied: static ... (...) => ... ;  (has arrow)
    return has_static and has_paren and (not has_semicolon or has_arrow)


def extract_extension_method(signature_text: str) -> Optional[Tuple[str, int]]:
    if "this " not in signature_text:
        return None

    param_start = signature_text.find("(")
    if param_start == -1:
        return None

    prefix = signature_text[:param_start].strip()
    if not prefix:
        return None

    # Find method name: extract last identifier from prefix
    # Work backwards to find the method name before any generics or whitespace
    stripped = prefix.rstrip()
    if stripped.endswith(">"):
        # Has generic parameters, find the identifier before '<'
        angle_pos = stripped.rfind("<")
        if angle_pos > 0:
            stripped = stripped[:angle_pos].rstrip()

    # Now extract the last identifier
    match = re.search(r"([A-Za-z_]\w*)$", stripped)
    if not match:
        return None
    method_name = match.group(1)

    param_end = find_matching_paren(signature_text, param_start)
    params_text = signature_text[param_start + 1 : param_end] if param_end != -1 else ""
    total_param_count = count_params_in_signature(params_text)
    # Store count excluding 'this' parameter (total - 1)
    param_count_excluding_this = total_param_count - 1 if total_param_count > 0 else 0
    return method_name, param_count_excluding_this


def detect_static_class(
    line: str,
    namespace: Optional[str],
    current_class: Optional[Tuple[str, int]],
    pending_class: Optional[str],
    brace_depth: int,
    static_class_pattern: re.Pattern,
) -> Tuple[Optional[Tuple[str, int]], Optional[str]]:
    class_match = static_class_pattern.search(line)
    if class_match:
        class_name = class_match.group(1)
        pending_class = f"{namespace}.{class_name}" if namespace else class_name

    if pending_class and "{" in line:
        current_class = (pending_class, brace_depth)
        pending_class = None

    return current_class, pending_class


def update_signature_buffer(
    line: str,
    current_class: Optional[Tuple[str, int]],
    signature_buffer: List[str],
) -> Tuple[List[str], Optional[str]]:
    if current_class and is_method_candidate(line):
        signature_buffer = [line.strip()]
    elif signature_buffer:
        signature_buffer.append(line.strip())

    if signature_buffer:
        # Check if we have a complete signature by looking for matching parens
        signature_text = " ".join(signature_buffer)
        param_start = signature_text.find("(")
        if param_start != -1:
            param_end = find_matching_paren(signature_text, param_start)
            if param_end != -1:
                # Found matching closing paren, signature is complete
                return [], signature_text

    return signature_buffer, None


def close_class_if_needed(
    brace_depth: int, current_class: Optional[Tuple[str, int]]
) -> Optional[Tuple[str, int]]:
    if current_class and brace_depth <= current_class[1]:
        return None
    return current_class


def iter_extension_signatures(text: str) -> List[Tuple[str, str]]:
    namespace_pattern = re.compile(r"^\s*namespace\s+([\w.]+)\s*[;{]", re.MULTILINE)
    static_class_pattern = re.compile(
        r"\bstatic\s+(?:partial\s+)?class\s+([A-Za-z_]\w*)\b"
    )
    namespace = extract_namespace(text, namespace_pattern)
    brace_depth = 0
    current_class: Optional[Tuple[str, int]] = None
    pending_class: Optional[str] = None
    signature_buffer: List[str] = []
    signatures: List[Tuple[str, str]] = []

    for line in text.splitlines():
        current_class, pending_class = detect_static_class(
            line,
            namespace,
            current_class,
            pending_class,
            brace_depth,
            static_class_pattern,
        )

        signature_buffer, signature_text = update_signature_buffer(
            line, current_class, signature_buffer
        )
        if signature_text and current_class:
            signatures.append((current_class[0], signature_text))

        brace_depth += line.count("{") - line.count("}")
        current_class = close_class_if_needed(brace_depth, current_class)

    return signatures


def get_extension_method_map() -> Dict[Tuple[str, str, int], bool]:
    extension_methods: Dict[Tuple[str, str, int], bool] = {}
    for path in iter_files(SRC_ROOT, "*.cs"):
        try:
            text = path.read_text(encoding="utf-8", errors="ignore")
        except OSError:
            continue

        for class_name, signature_text in iter_extension_signatures(text):
            extracted = extract_extension_method(signature_text)
            if not extracted:
                continue
            method_name, param_count = extracted
            extension_methods[(class_name, method_name, param_count)] = True

    return extension_methods


def normalize_whitespace(value: str) -> str:
    return re.sub(r"\s+", " ", value or "").strip()


def convert_doc_xml(value: str) -> str:
    if not value:
        return ""
    text = value
    text = re.sub(r"<c>(.*?)</c>", r"`\1`", text, flags=re.DOTALL)
    text = re.sub(r"<paramref name=\"(.*?)\"\s*/>", r"`\1`", text)
    text = re.sub(r"<typeparamref name=\"(.*?)\"\s*/>", r"`\1`", text)
    text = re.sub(r"<para>\s*", "\n\n", text)
    text = text.replace("</para>", "")
    text = re.sub(r"<br\s*/?>", "\n", text)

    def replace_see(match: re.Match) -> str:
        cref = match.group(1) or ""
        cref = re.sub(r"^[A-Z]:", "", cref)
        cref = cref.replace("#", "").replace("``", "")
        return f"`{cref}`"

    text = re.sub(r"<see cref=\"(.*?)\"\s*/>", replace_see, text)
    text = re.sub(r"<[^>]+>", "", text)
    return normalize_whitespace(text)


def split_params(param_block: str) -> List[str]:
    if not param_block:
        return []
    params: List[str] = []
    current = []
    depth = 0
    for char in param_block:
        if char == "{" or char == "[":
            depth += 1
        elif char == "}" or char == "]":
            depth -= 1
        elif char == "," and depth == 0:
            params.append("".join(current).strip())
            current = []
            continue
        current.append(char)
    if current:
        params.append("".join(current).strip())
    return params


def format_type(type_name: str) -> str:
    if not type_name:
        return ""

    type_name = type_name.replace("@", "")

    if type_name in TYPE_ALIASES:
        return TYPE_ALIASES[type_name]

    method_generic_match = re.fullmatch(r"``(\d+)", type_name)
    if method_generic_match:
        return f"T{int(method_generic_match.group(1)) + 1}"

    type_generic_match = re.fullmatch(r"`(\d+)", type_name)
    if type_generic_match:
        return f"T{int(type_generic_match.group(1)) + 1}"

    if type_name.startswith("System.Nullable{") and type_name.endswith("}"):
        inner = format_type(type_name[len("System.Nullable{") : -1])
        return f"{inner}?"

    if "{" in type_name and type_name.endswith("}"):
        outer, inner_block = type_name.split("{", 1)
        inner_block = inner_block[:-1]
        outer = format_type(outer)
        inner_types = split_params(inner_block)
        inner = ", ".join(format_type(item) for item in inner_types)
        return f"{outer}<{inner}>"

    if type_name.endswith("[]"):
        return f"{format_type(type_name[:-2])}[]"

    base_name = type_name.split(".")[-1].replace("+", ".")
    if "`" in base_name:
        name, _, count_str = base_name.partition("`")
        try:
            count = int(count_str or 0)
        except ValueError:
            return name
        generics = ", ".join(f"T{index}" for index in range(1, count + 1))
        return f"{name}<{generics}>"
    return base_name


def format_type_display(type_name: str) -> str:
    base_name = type_name.replace("+", ".").split(".")[-1]
    if "`" in base_name:
        name, _, count_str = base_name.partition("`")
        try:
            count = int(count_str or 0)
        except ValueError:
            return name
        generics = ", ".join(f"T{index}" for index in range(1, count + 1))
        return f"{name}<{generics}>"
    return base_name


def format_param_list(param_types: List[str]) -> str:
    return ", ".join(format_type(param_type) for param_type in param_types)


def get_method_display_name(name: str) -> str:
    if "``" in name:
        base, _, count_str = name.partition("``")
        count = int(count_str or 0)
        generics = ", ".join(f"T{index}" for index in range(1, count + 1))
        return f"{base}<{generics}>"
    return name


def placeholder_for_type(type_name: str) -> str:
    simplified = format_type(type_name)
    if simplified.endswith("?"):
        return f"default({simplified})"

    direct = {
        "string": "\"value\"",
        "String": "\"value\"",
        "bool": "true",
        "Boolean": "true",
        "char": "'a'",
        "CancellationToken": "CancellationToken.None",
        "Type": "typeof(object)",
        "Exception": "new Exception(\"message\")",
    }
    if simplified in direct:
        return direct[simplified]

    numeric = {"byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong"}
    if simplified in numeric:
        return "1"

    floating = {"float", "double", "decimal"}
    if simplified in floating:
        return "1.0"

    if simplified.endswith("[]"):
        element = simplified[:-2]
        return f"Array.Empty<{element}>()"

    collection_prefixes = (
        "IEnumerable<",
        "IReadOnlyCollection<",
        "ICollection<",
        "IReadOnlyList<",
        "IList<",
    )
    for prefix in collection_prefixes:
        if simplified.startswith(prefix):
            inner = simplified[len(prefix) : -1]
            return f"Array.Empty<{inner}>()"

    if simplified.startswith("Func<") or simplified.startswith("Action<"):
        return f"default({simplified})"

    if simplified.startswith(("Dictionary<", "List<")):
        return f"new {simplified}()"

    if simplified.startswith("T"):
        return f"default({simplified})"

    return f"default({simplified})"


def build_example(
    type_name: str,
    method_name: str,
    param_types: List[str],
    is_static_type: bool,
    has_return: bool,
    is_constructor: bool,
    is_extension: bool,
) -> str:
    type_display = format_type_display(type_name)
    args = ", ".join(placeholder_for_type(param) for param in param_types)

    if is_constructor:
        return f"var instance = new {type_display}({args});"

    if is_extension and param_types:
        receiver = placeholder_for_type(param_types[0])
        extension_args = ", ".join(placeholder_for_type(param) for param in param_types[1:])
        call = f"{receiver}.{method_name}({extension_args})"
        return f"var result = {call};" if has_return else f"{call};"

    call_target = type_display if is_static_type else "instance"
    call = f"{call_target}.{method_name}({args})"
    if has_return:
        call = f"var result = {call};"

    if is_static_type:
        return call

    return f"var instance = new {type_display}(/* ... */);\n{call}"


def parse_member_name(name: str) -> Tuple[str, str, Optional[str], Optional[str]]:
    kind = name[0]
    content = name[2:]
    param_block = None
    if "(" in content:
        content, param_block = content.split("(", 1)
        param_block = param_block.rstrip(")")
    if kind == "T":
        return kind, content, None, None
    type_name, member_name = content.rsplit(".", 1)
    return kind, type_name, member_name, param_block


def get_member_text(member: ET.Element, tag: str) -> str:
    node = member.find(tag)
    xml = ET.tostring(node if node is not None else ET.Element(tag), encoding="unicode")
    return convert_doc_xml(xml)


def parse_params(member: ET.Element) -> List[Dict[str, str]]:
    params = []
    for param in member.findall("param"):
        params.append(
            {
                "name": param.attrib.get("name", ""),
                "description": convert_doc_xml(ET.tostring(param, encoding="unicode")),
            }
        )
    return params


def parse_exceptions(member: ET.Element) -> List[Dict[str, str]]:
    exceptions = []
    for exception in member.findall("exception"):
        exceptions.append(
            {
                "cref": exception.attrib.get("cref", ""),
                "description": convert_doc_xml(ET.tostring(exception, encoding="unicode")),
            }
        )
    return exceptions


def collect_assemblies() -> Dict[str, Dict[str, Dict[str, object]]]:
    assemblies: Dict[str, Dict[str, Dict[str, object]]] = defaultdict(lambda: defaultdict(dict))

    for xml_path in iter_files(SRC_ROOT, "*.xml"):
        if not xml_path.name.startswith("Ploch."):
            continue

        try:
            tree = ET.parse(xml_path)
        except ET.ParseError:
            continue

        root = tree.getroot()
        assembly_name = root.findtext("./assembly/name") or xml_path.stem

        for member in root.findall("./members/member"):
            name_attr = member.attrib.get("name", "")
            if not name_attr or len(name_attr) < 2:
                continue

            kind, type_name, member_name, param_block = parse_member_name(name_attr)
            if kind == "T":
                type_entry = assemblies[assembly_name].setdefault(type_name, {})
                type_entry["summary"] = get_member_text(member, "summary")
                type_entry["remarks"] = get_member_text(member, "remarks")
                type_entry.setdefault("members", [])
                continue

            type_entry = assemblies[assembly_name].setdefault(type_name, {})
            type_entry.setdefault("members", [])
            type_entry["members"].append(
                {
                    "kind": kind,
                    "member_name": member_name,
                    "param_block": param_block,
                    "summary": get_member_text(member, "summary"),
                    "remarks": get_member_text(member, "remarks"),
                    "returns": get_member_text(member, "returns"),
                    "example": get_member_text(member, "example"),
                    "params": parse_params(member),
                    "exceptions": parse_exceptions(member),
                }
            )

    return assemblies


def build_method_signature(
    type_display: str, member: Dict[str, object]
) -> Tuple[str, str, List[str], bool]:
    raw_name = member["member_name"]
    is_constructor = raw_name == "#ctor"
    method_name = get_method_display_name(type_display if is_constructor else raw_name)
    param_types = split_params(member.get("param_block") or "")
    signature = f"{method_name}({format_param_list(param_types)})"
    return signature, method_name, param_types, is_constructor


def render_method(
    type_name: str,
    type_display: str,
    member: Dict[str, object],
    static_types: Dict[str, bool],
    extension_methods: Dict[Tuple[str, str, int], bool],
) -> List[str]:
    signature, method_name, param_types, is_constructor = build_method_signature(
        type_display, member
    )
    lines: List[str] = []
    lines.append("")
    lines.append(f"##### `{signature}`")
    lines.append("")

    if member["summary"]:
        lines.append(member["summary"])
        lines.append("")

    if member["remarks"]:
        lines.append(f"Remarks: {member['remarks']}")
        lines.append("")

    if member["params"]:
        lines.append("**Parameters**")
        for param in member["params"]:
            description = param["description"]
            lines.append(f"- `{param['name']}`: {description}")
        lines.append("")

    if member["returns"]:
        lines.append(f"**Returns**: {member['returns']}")
        lines.append("")

    if member["exceptions"]:
        lines.append("**Exceptions**")
        for exception in member["exceptions"]:
            cref = exception["cref"].replace("T:", "")
            description = exception["description"]
            lines.append(f"- `{cref}`: {description}")
        lines.append("")

    example = member["example"]
    if not example:
        member_base_name = member["member_name"].split("``")[0]
        # Extension method map counts params AFTER 'this', so subtract 1 from XML param count
        extension_param_count = len(param_types) - 1 if len(param_types) > 0 else 0
        is_extension = (
            extension_methods.get((type_name, member_base_name, extension_param_count), False)
            and len(param_types) > 0
        )
        example = build_example(
            type_name,
            method_name,
            param_types,
            static_types.get(type_name, False),
            bool(member["returns"]),
            is_constructor,
            is_extension,
        )

    lines.append("Example:")
    lines.append("")
    lines.append("```csharp")
    lines.append(example)
    lines.append("```")
    return lines


def render_type_section(
    type_name: str,
    type_entry: Dict[str, object],
    static_types: Dict[str, bool],
    extension_methods: Dict[Tuple[str, str, int], bool],
) -> List[str]:
    namespace = ".".join(type_name.split(".")[:-1])
    namespace_prefix = ".".join(type_name.replace("+", ".").split(".")[:-1])
    short_name = format_type_display(type_name)
    type_display = f"{namespace_prefix}.{short_name}" if namespace_prefix else short_name

    lines: List[str] = []
    lines.append("")
    lines.append(f"### {type_display}")
    lines.append("")
    if namespace:
        lines.append(f"**Namespace**: `{namespace}`")
        lines.append("")
    summary = type_entry.get("summary", "")
    if summary:
        lines.append(summary)
        lines.append("")
    remarks = type_entry.get("remarks", "")
    if remarks:
        lines.append(f"Remarks: {remarks}")
        lines.append("")

    members = [item for item in type_entry.get("members", []) if item["kind"] == "M"]
    if not members:
        lines.append("No public methods documented.")
        return lines

    lines.append("#### Methods")
    for member in sorted(members, key=lambda value: value["member_name"].lower()):
        lines.extend(render_method(type_name, type_display, member, static_types, extension_methods))

    return lines


def render_assembly_section(
    assembly: str,
    type_map: Dict[str, Dict[str, object]],
    static_types: Dict[str, bool],
    extension_methods: Dict[Tuple[str, str, int], bool],
) -> List[str]:
    lines: List[str] = []
    lines.append("")
    lines.append(f"## {assembly}")
    for type_name in sorted(type_map.keys(), key=lambda value: value.lower()):
        lines.extend(
            render_type_section(type_name, type_map[type_name], static_types, extension_methods)
        )
    return lines


def render_api_reference(
    assemblies: Dict[str, Dict[str, Dict[str, object]]],
    static_types: Dict[str, bool],
    extension_methods: Dict[Tuple[str, str, int], bool],
) -> List[str]:
    lines: List[str] = []
    lines.append("# Ploch.Common API Reference")
    lines.append("")
    lines.append("Comprehensive reference documentation for all public APIs in the Ploch.Common library suite.")
    lines.append("Generated from XML documentation in the `src` folder.")
    lines.append("")
    lines.append(f"Last updated: {datetime.date.today().isoformat()}")
    lines.append("")
    lines.append("## Assemblies")

    assembly_names = sorted(assemblies.keys(), key=lambda value: value.lower())
    for assembly in assembly_names:
        anchor = assembly.lower().replace(".", "")
        lines.append(f"- [{assembly}](#{anchor})")

    for assembly in assembly_names:
        lines.extend(
            render_assembly_section(
                assembly, assemblies[assembly], static_types, extension_methods
            )
        )

    return lines


def build_api_reference() -> None:
    static_types = get_static_type_map()
    assemblies = collect_assemblies()
    extension_methods = get_extension_method_map()
    lines = render_api_reference(assemblies, static_types, extension_methods)
    DOCS_PATH.write_text("\n".join(lines) + "\n", encoding="utf-8")


if __name__ == "__main__":
    build_api_reference()
