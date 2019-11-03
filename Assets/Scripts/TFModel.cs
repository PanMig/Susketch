using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tensorflow;
using static Tensorflow.Binding;
using NumSharp;

public class TFModel : MonoBehaviour
{
    public float[] PredictDeathHeatmap(NDArray map, NDArray weapons)
    {
        var pbFile = "death_heatmap.bytes";
        float[] heatmap = new float[16];
        // import GraphDef from pb file
        var graph = new Graph().as_default();
        graph.Import(Application.dataPath + "/" + pbFile);

        Tensor input_maps = graph.OperationByName("input_layer");
        Tensor input_weapons = graph.OperationByName("input_1");
        Tensor output = graph.OperationByName("output_layer/BiasAdd");

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

    public float PredictDramaticArc(NDArray map, NDArray weapons)
    {
        var pbFile = "dramatic_arc.bytes";
        float[] dramatic_arc = new float[5];
        // import GraphDef from pb file
        var graph = new Graph().as_default();
        graph.Import(Application.dataPath + "/" + pbFile);

        Tensor input_maps = graph.OperationByName("input_layer");
        Tensor input_weapons = graph.OperationByName("input_1");
        Tensor output = graph.OperationByName("output_0/BiasAdd");
        Tensor output2 = graph.OperationByName("output_1/BiasAdd");
        Tensor output3 = graph.OperationByName("output_2/BiasAdd");
        Tensor output4 = graph.OperationByName("output_3/BiasAdd");
        Tensor output5 = graph.OperationByName("output_4/BiasAdd");

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

            var value = (output_1*20) + (output_2 * 20) + (output_3 *20) + (output_4*20) + (output_5*20);
            var x = value.ToArray<float>()[0];
            return x;
        }
    }



}
