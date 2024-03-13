
//namespace CSharpLike
//{
//    /// <summary>
//    /// Packet type between client and server.
//    /// The prefix without 'CB_' mean client send to sever,
//    /// otherwise mean server send to client.
//    /// </summary>
//    public enum PacketType
//    {
//        //Common
//        /// <summary>callback object(s) to client, DON'T change this name or value due to KissFramework had bound it</summary>
//        CB_Object = 0,
//        /// <summary>callback delete object(s) to client, DON'T change this name or value due to KissFramework had bound it</summary>
//        CB_Delete = 1,
//        /// <summary>callback common error message to client</summary>
//        CB_Error,
//        /// <summary>callback common tips to client</summary>
//        CB_Tips,
//        /// <summary>Get common reward</summary>
//        CB_GetReward,

//        //Account system
//        AccountLogin = 100,
//        CB_AccountLogin,
//        AccountChangeNameAndPassword,
//        CB_AccountChangeNameAndPassword,
//        CB_KickAccount,

//        //Item system
//        UseItem = 200,
//        CB_UseItem,

//        //Mail system
//        ReadMail = 300,
//        TakeMailAppendix,
//        DeleteMail,

//        //SignIn system
//        SignInForGift = 400,
//    }
//}
