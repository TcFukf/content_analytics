using social_analytics.Bl;

internal interface IHavePlustOperation<TRes> where TRes:IHavePlustOperation<TRes>
{
    TRes Plus(TRes add);
    TRes Value();
}