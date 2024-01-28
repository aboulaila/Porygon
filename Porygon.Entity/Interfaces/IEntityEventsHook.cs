namespace Porygon.Entity.Interfaces
{
    public interface IEntityEventsHook
	{
        public Task InvokePreValidation(object model);
        public Task InvokePre(object model);
        public Task InvokePost(object model);
    }
}

