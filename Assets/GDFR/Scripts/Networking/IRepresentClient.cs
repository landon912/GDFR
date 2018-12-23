﻿public interface IRepresentClient
{
    bool IsRepresentingClientId(int clientId);
    void SetAsRepresentingClientId(int clientId);
    bool IsRepresentingNetworkClient();
    bool IsAI();
}