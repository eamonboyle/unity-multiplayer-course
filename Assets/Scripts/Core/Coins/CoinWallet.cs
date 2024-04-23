using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent(out Coin coinComponent))
        {
            return;
        }

        int coinValue = coinComponent.Collect();

        if (!IsServer)
        {
            return;
        }

        TotalCoins.Value += coinValue;
    }

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }
}
