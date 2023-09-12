namespace Ploch.Common.Data.Model;

/// <inheritdoc cref="IAuditableEntity" />
public interface IAuditableEntity<TId> : IAuditableEntity, IHasIdSettable<TId>
{ }

/// <summary>
///     An entity with audit properties.
/// </summary>
/// <remarks>
///     An entity with properties used in auditing like created or modified time and user.
/// </remarks>
public interface IAuditableEntity : IHasCreatedTime, IHasCreatedBy, IHasModifiedTime, IHasModifiedBy, IHasAccessedTime, IHasAccessedBy
{ }