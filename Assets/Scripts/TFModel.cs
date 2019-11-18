using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tensorflow;
using static Tensorflow.Binding;
using NumSharp;
using static TileMapLogic.TileMap;
using TileMapLogic;


public class TFModel : MonoBehaviour
{
    private static Graph heatmapGraph;
    private static Graph dArcGraph;
    private static Graph combatPaceGraph;
    private static Graph killRatioGraph;

    public void Start()
    {
        heatmapGraph = InitGraph(heatmapGraph, "death_heatmap");
        killRatioGraph = InitGraph(heatmapGraph, "kill_ratio");
        dArcGraph = InitGraph(dArcGraph, "dramatic_arc");
    }

    private Graph InitGraph(Graph graph, string pbFile)
    {
        graph = new Graph();
        var model_file = Resources.Load<TextAsset>(pbFile).bytes;
        graph.Import(model_file);
        return graph;
    }

    public static float[] PredictDeathHeatmap(NDArray map, NDArray weapons)
    {
        heatmapGraph.as_default();
        Tensor input_maps = heatmapGraph.OperationByName("input_layer");
        Tensor input_weapons = heatmapGraph.OperationByName("input_1");
        Tensor output = heatmapGraph.OperationByName("output_layer/BiasAdd");

        using (var sess = tf.Session())
        {
            var results = sess.run(output, new FeedItem[]
            {
                new FeedItem(input_maps, map),
                new FeedItem(input_weapons, weapons)
            });

            var x = results.ToArray<float>();
            return x;
        }
    }

    public static float PredictKillRatio(NDArray map, NDArray weapons)
    {
        map = ConcatCoverChannel(map);
        killRatioGraph.as_default();
        Tensor input_maps = killRatioGraph.OperationByName("input_layer");
        Tensor input_weapons = killRatioGraph.OperationByName("input_12");
        Tensor output = killRatioGraph.OperationByName("output_layer/BiasAdd");

        using (var sess = tf.Session())
        {
            var results = sess.run(output, new FeedItem[]
            {
                new FeedItem(input_maps, map),
                new FeedItem(input_weapons, weapons)
            });

            var x = results.ToArray<float>();
            return x[0];
        }
    }

    public static float PredictDramaticArc(NDArray map, NDArray weapons)
    {
        dArcGraph.as_default();
        Tensor input_maps = dArcGraph.OperationByName("input_layer");
        Tensor input_weapons = dArcGraph.OperationByName("input_1");
        Tensor output = dArcGraph.OperationByName("output_0/BiasAdd");
        Tensor output2 = dArcGraph.OperationByName("output_1/BiasAdd");
        Tensor output3 = dArcGraph.OperationByName("output_2/BiasAdd");
        Tensor output4 = dArcGraph.OperationByName("output_3/BiasAdd");
        Tensor output5 = dArcGraph.OperationByName("output_4/BiasAdd");

        using (var sess = tf.Session())
        {
            var output_1 = sess.run(output, new FeedItem[]
            {
                new FeedItem(input_maps, map),
                new FeedItem(input_weapons, weapons)
            });

            var output_2 = sess.run(output2, new FeedItem[]
            {
                new FeedItem(input_maps, map),
                new FeedItem(input_weapons, weapons)
            });

            var output_3 = sess.run(output3, new FeedItem[]
            {
                new FeedItem(input_maps, map),
                new FeedItem(input_weapons, weapons)
            });

            var output_4 = sess.run(output4, new FeedItem[]
            {
                new FeedItem(input_maps, map),
                new FeedItem(input_weapons, weapons)
            });

            var output_5 = sess.run(output5, new FeedItem[]
            {
                new FeedItem(input_maps, map),
                new FeedItem(input_weapons, weapons)
            });

            var value = output_1 + output_2 + output_3 + output_4 + output_5;
            var x = value.ToArray<float>()[0];
            return x;
        }
    }

    public static NDArray GetInputWeapons(CharacterParams blueClass, CharacterParams redClass)
    {
        var blue = blueClass.class_params;
        var red = redClass.class_params;

        // concat two arrays (first red then blue)
        var merged = new double[blue.Length + red.Length];
        red.CopyTo(merged, 0);
        blue.CopyTo(merged, blue.Length);

        NDArray arr = new NDArray(merged);
        var input_weapons = np.array(arr);

        input_weapons = np.expand_dims(input_weapons, 0);
        return input_weapons;
    }

    public static NDArray GetInputMap(TileMap tileMap)
    {
        var map = tileMap.GetTileMapToString();
        var input_map = ArrayParsingUtils.ParseToChannelArray(map);
        input_map = np.expand_dims(input_map, 0);
        return input_map;
    }

    public static NDArray ConcatCoverChannel(NDArray input_map)
    {
        var coverChannel = np.zeros(1, 20, 20, 1);
        input_map = np.concatenate(new NDArray[2] { input_map, coverChannel }, 3);
        return input_map;
    }


}
