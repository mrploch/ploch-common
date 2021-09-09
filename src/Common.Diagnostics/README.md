# Ploch.Common.Diagnostics Project

## Overview

Collection of diagnostic-related utility classes.

## features

### Record / Replay

[OperationRecorder](OperationRecorder.cs) class provides an easy way of recording the state of an operation.

It serializes values provided in an `Expression` to JSON and saves it on disk.
