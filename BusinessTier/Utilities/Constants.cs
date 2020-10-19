namespace BusinessTier.Utilities
{
    public static class Constants
    {
        public const string SecretKey = "ThisisareallylonglonglonglongEDMSecretkey";
        public const string Issuer = "EDM";

        public const int ROLE_STAFF_ID = 2;
        public const string ROLE_ADMIN_NAME = "Adminstrator";
        public const string ROLE_MOD_NAME = "Moderator";


        public const string ERR_UNAME_NOTAVAILABLE = "ERR_UNAME_NOTAVAILABLE";//trùng username
        public const string ERR_PWD_NOTMATCH = "ERR_PWD_NOTMATCH";// pwd confirmation != pwd
        public const string ERR_EMPTY_ID = "ERR_EMPTY_ID";//id rỗng
        public const string ERR_EMPTY_PWD = "ERR_EMPTY_PWD";//pwd rỗng
        public const string ERR_EMPTY_ROOMNUM = "ERR_EMPTY_ROOMNUM";//room number rỗng
        public const string ERR_EMPTY_DNAME = "ERR_EMPTY_DNAME"; //department name rỗng
        public const string ERR_EMPTY_PWD_CONFIR = "ERR_EMPTY_PWD_CONFIR";//pwd confirmation rỗng
        public const string ERR_EMPTY_EMAIL = "ERR_EMPTY_EMAIL";//email rỗng
        public const string ERR_EMPTY_FNAME = "ERR_EMPTY_FNAME";//full name rỗng
        public const string ERR_ROLE_FK = "ERR_ROLE_FK";//foreign key role
        public const string ERR_PK_EXIST = "ERR_PK_EXIST";//trùng id primary key
    }
}
