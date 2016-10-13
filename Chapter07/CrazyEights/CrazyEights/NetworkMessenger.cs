using System;
using Microsoft.Xna.Framework.Net;
using GameStateManager;
using CardLib;

namespace CrazyEights
{
    public enum NetworkMessageType : byte
    {
        HostSendPlayer,
        HostDealCard,
        HostAllCardsDealt,
        HostDiscard,
        HostSetTurn,
        PlayCard,
        RequestCard,
        SuitChosen,
        GameWon
    }

    public static class NetworkMessenger
    {
        private static ScreenManager screenManager;
        private static bool isInitialized = false;

        public static void Host_SendPlayer(NetworkGamer gamer)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.HostSendPlayer);
            screenManager.Network.PacketWriter.Write(gamer.Gamertag);
            screenManager.Network.PacketWriter.Write(gamer.IsHost);
            SendData();
        }

        public static void Host_DealCard(Card card, CrazyEightsPlayer player)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.HostDealCard);
            screenManager.Network.PacketWriter.Write(player.Name);
            screenManager.Network.PacketWriter.Write(card.Serialize());            
            SendData();
        }

        public static void Host_Discard(Card card)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.HostDiscard);
            screenManager.Network.PacketWriter.Write(card.Serialize());            
            SendData();
        }

        public static void Host_ReadyToPlay()
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.HostAllCardsDealt);
            SendData();
        }

        public static void Host_SetTurn(int turn)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.HostSetTurn);
            screenManager.Network.PacketWriter.Write(turn);
            SendData();
        }

        public static void PlayCard(Card card)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.PlayCard);
            screenManager.Network.PacketWriter.Write(card.Serialize());
            SendData();
        }

        public static void RequestCard(CrazyEightsPlayer player)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.RequestCard);
            screenManager.Network.PacketWriter.Write(player.Name);
            SendData();
        }

        public static void SendChosenSuit(Suit suit)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.SuitChosen);
            screenManager.Network.PacketWriter.Write((byte)suit);
            SendData();
        }

        public static void GameWon(string winner)
        {
            screenManager.Network.PacketWriter.Write((byte)NetworkMessageType.GameWon);
            screenManager.Network.PacketWriter.Write(winner);
            SendData();
        }

        public static void Initialize(ScreenManager manager)
        {
            screenManager = manager;
            isInitialized = true;
        }        

        private static void SendData()
        {
            if (isInitialized == false)
                throw new Exception("This object must be initialized first.");

            foreach (LocalNetworkGamer gamer in screenManager.Network.Session.LocalGamers)
                gamer.SendData(screenManager.Network.PacketWriter, SendDataOptions.ReliableInOrder);
        }       
    }
}