namespace wema_test_service.Common.Enums;

public enum CustomerStatusEnum
{
    Created = 1,
    Active,
    Inactive,
    Deleted
}

public enum CustomerVerificationStatusEnum
{
    Pending = 1,
    Verified,
    NotVerified
}

public enum OtpStatusEnum
{
    Created = 1,
    Sent,
    Verified,
    Invalidated,
    Expired
}
