using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NumSharp;
using System.Threading.Tasks;
using static TFModel;

public class MLSuggestionsMng : MonoBehaviour
{
    private enum pickups { health, armor, damage, none};

    public static Task<CharacterParams[]> GetBalancedMatchup(List<CharacterParams[]> classMatchups, NDArray input_map)
    {
        return Task.Run(() =>
        {
            //expand maps dimension by one.
            input_map = ConcatCoverChannel(input_map);

            var classes = new CharacterParams[2];
            var desiredBalancedVal = 0.5f;
            float score = 0;
            var scores = new List<float>();
            foreach (var matchup in classMatchups)
            {
                var input_weapons = GetInputWeapons(matchup[0], matchup[1]);
                score = TFModel.PredictKillRatio(input_map, input_weapons);
                scores.Add(score);
            }

            var closest = scores.Aggregate((x, y) => Mathf.Abs(x - desiredBalancedVal) < Mathf.Abs(y - desiredBalancedVal) ? x : y);
            var resultIdx = scores.IndexOf(closest);

            classes[0] = classMatchups[resultIdx][0];
            classes[1] = classMatchups[resultIdx][1];

            return classes;
        });
    }

    //public static Task<float[][]> SpawnBalancedPickUps()
    //{
    //    List<float> balanceScores = new List<float>();
    //    var desiredBalancedVal = 0.5f;

    //    return Task.Run(() =>
    //    {
    //        // randomly select a region to spawn a pickup.
    //        for (int i = 1; i < 5; i++)
    //        {
    //            for (int j = 1; j < 5; j++)
    //            {
    //                int diceRoll = Random.Range(0, 3);

    //            }
    //        }

    //        var closest = balanceScores.Aggregate((x, y) => Mathf.Abs(x - desiredBalancedVal) < Mathf.Abs(y - desiredBalancedVal) ? x : y);
    //        var resultIdx = balanceScores.IndexOf(closest);

    //    });
    //}
}
