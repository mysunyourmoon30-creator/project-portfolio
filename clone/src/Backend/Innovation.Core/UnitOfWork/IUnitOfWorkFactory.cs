namespace Innovation.Core.UnitOfWork;

// THE central fix of Phase 1. In the original system, every service's
// constructor called a public *static* UnitOfWorkFactory
// (Backend ROADMAP §4.2/§6.3/§7b): `_uow = UnitOfWorkFactory.GetDBTransectionUnitOfWork();`
// DI resolved the service, but the service then reached past DI to grab its
// own dependency from a static class - so nothing that touched the database
// could be unit tested or mocked.
//
// Here the factory is an ordinary interface, injected through the
// constructor like anything else, registered as
// services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>(). Any consumer
// can now be tested by substituting this interface - see
// UnitOfWorkFactoryTests.Consumer_WithInjectedFactory_CanBeConstructedAndTested_WithoutTouchingRealDatabase.
public interface IUnitOfWorkFactory
{
    ISiloUnitOfWork CreateSiloUnitOfWork();
}
