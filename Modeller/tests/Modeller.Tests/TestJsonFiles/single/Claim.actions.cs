namespace Jbssa.ASAP
{
    partial class Claim
    {
        private Claim() { }
        public static Claim Create()
        {
            return new Claim { IsActive = true };
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
