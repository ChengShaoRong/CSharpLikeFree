
//namespace CSharpLike
//{
//    /// <summary>
//    /// Packet type between client and server.
//    /// 'CB' is the abbreviation of 'Callback'.
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

//        //Account system
//        AccountLogin = 100,
//        CB_AccountLogin,
//        AccountChangeNameAndPassword,
//        CB_AccountChangeNameAndPassword,
//        CB_KickAccount,

//        //Chat room system
//        ChatRoomEnter = 200,
//        CB_ChatRoomEnter,
//        ChatRoomExit,
//        CB_ChatRoomExit,
//        ChatRoomSend,
//        CB_ChatRoomSend,
//        CB_HistoryMsg,

//        //Aircraft battle system
//        AircraftBattleSendScore = 300,
//        CB_AircraftBattleSendScore,
//        AircraftBattleGetRank,
//        CB_AircraftBattleGetRank,

//        //Item system
//        UseItem = 400,
//        CB_UseItem,

//        //Mail system
//        ReadMail = 500,
//        CB_ReadMail,
//        TakeMailAppendix,
//        CB_TakeMailAppendix,

//        //SignIn system
//        SignInForGift = 600,
//        CB_SignInInForGift,
//        SignInForGiftVIP,
//        CB_SignInInForGiftVIP,
//    }
//}
