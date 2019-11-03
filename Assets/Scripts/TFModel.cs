using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tensorflow;
using static Tensorflow.Binding;
using NumSharp;

public class TFModel : MonoBehaviour
{
    public float[] Predict(NDArray map, NDArray weapons)
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
    


}
