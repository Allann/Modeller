namespace Jbssa.ASAP
{
    partial class Claim
    {
        partial void GetStringValue(ref string value);
        private Claim() { }
        public static Claim Create()
        {
            Claim claim = null;
            CreateEntity(ref claim);
            if (claim == null)
            {
                claim = new Claim { IsActive = true };
            }
            return claim;
        }
        public static Claim Create(string name, string claimValueType, bool allowMultipleInstance, bool alwaysIncludeInIdToken, bool isResourceValue, bool isRoleValue, bool isUserValue)
        {
            var result = Create();
            result.Name = name;
            result.ClaimValueType = claimValueType;
            result.AllowMultipleInstance = allowMultipleInstance;
            result.AlwaysIncludeInIdToken = alwaysIncludeInIdToken;
            result.IsResourceValue = isResourceValue;
            result.IsRoleValue = isRoleValue;
            result.IsUserValue = isUserValue;
            return result;
        }
    }
}
