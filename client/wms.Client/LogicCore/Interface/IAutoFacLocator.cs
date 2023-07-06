namespace wms.Client.LogicCore.Interface
{
    public interface IAutoFacLocator
    {
        void Register();

        TInterface Get<TInterface>();

        TInterface Get<TInterface>(string typeName);

    }
}
