using System.Collections;
using System.Collections.Generic;
using TMPro;
using FishNet.Object;
using FishNet.Connection;

public class Score : NetworkBehaviour
{
    private Dictionary<NetworkConnection, int> playerscore = new Dictionary<NetworkConnection, int>();
    private TextMeshProUGUI scoreTMP;

    public override void OnStartClient()
    {
        base.OnStartClient();
        scoreTMP = GetComponent<TextMeshProUGUI>();
        scoreTMP.text = "score: " + 0;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CmdAddNewPlayer(NetworkConnection sender = null) => playerscore.Add(sender, 0);

    [Server]
    public void ChangePoint(NetworkConnection id, int change)
    {
        playerscore[id] += change;
        TargetChangeScoreOnPlayer(id, playerscore[id]);
    }
    [TargetRpc]
    private void TargetChangeScoreOnPlayer(NetworkConnection target, int score) => scoreTMP.text = "score: " + score;
}