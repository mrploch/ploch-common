## `Ploch.Common.WebApi`: controller-based operation ids are now unique

**Fixed colliding OpenAPI `operationId`s for MVC controller endpoints.**
`OpenApiConfigurator.BuildOperationId` built the id for a controller action as controller
name plus HTTP method, which is not unique whenever one controller exposes more than one
route for the same verb — the ordinary shape of a REST controller:

| Route | operationId (before) |
|---|---|
| `GET /orders` | `OrdersGET` |
| `GET /orders/{id}` | `OrdersGET` |
| `GET /orders/{id}/items` | `OrdersGET` |

OpenAPI requires `operationId` to be unique across the document. Duplicates make client
generators either fail outright or silently drop operations, so a generated client could
end up missing endpoints with no error reported.

Controller ids now carry the same deterministic, route-derived suffix the route-based
fallback branch already used (a truncated SHA-256 of the route, so it is stable across
processes and runs and generated documents stay diffable). Uniqueness therefore follows
from the route by construction rather than from attempting to detect collisions, and the
controller name stays at the front of the id for readability:

| Route | operationId (after) |
|---|---|
| `GET /orders` | `OrdersGET_1c168adb` |
| `GET /orders/{id}` | `OrdersGET_ef7dbd91` |
| `GET /orders/{id}/items` | `OrdersGET_920da46d` |

When an `ApiExplorer` provider does not populate `RelativePath`, the `action` route value
is used as the discriminator instead; with neither available the id keeps its previous
`{Controller}{Method}` form.

### Compatibility

This changes the generated `operationId` for every controller-based endpoint, and so the
member names produced by OpenAPI client generators. `Ploch.Common.WebApi` has never been
published (`IsPackable=false`), so there are no package consumers to break; anyone
generating a client from a locally built document will see renamed operations. Whether to
ship the package is tracked in #285.

Refs: #298
