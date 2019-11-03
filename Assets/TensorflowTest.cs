using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Tensorflow;
using static Tensorflow.Binding;
using NumSharp;

public class TensorflowTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Run_BasicExamples();
        load_model();
    }

    public void Run_BasicExamples()
    {
        var c1 = tf.constant(3);
        var v1 = tf.Variable("I am variable", name: "v1");
        var p1 = tf.placeholder(tf.int32);
        var p2 = tf.placeholder(tf.int32);

        // Define some operations
        var add = tf.add(p1, p2);
        var mul = tf.multiply(c1, c1);

        using (var sess = tf.Session())
        {
            //// constants
            var result = sess.run(c1);
            Debug.Log(result);
        }

        using (var sess = tf.Session())
        {
            //// variables
            sess.run(v1.initializer);
            var result = sess.run(v1);
            Debug.Log(result);
        }

        using (var sess = tf.Session())
        {


            // placeholders
            var result = sess.run(add, feed_dict: new FeedItem[]
            {
                new FeedItem(p1, 2),
                new FeedItem(p2,2)
            });
            Debug.Log(result);


        }
    }

    public void load_model()
    {
        var pbFile = "death_heatmap.bytes";
        var map_in = tf.placeholder(tf.float32, (1, 20, 20, 7));
        var weapons_in = tf.placeholder(tf.float32, (1, 16));

        var maps = np.ones((1, 20, 20, 7));
        var weapons = np.zeros((1, 16));

        // import GraphDef from pb file
        var graph = new Graph().as_default();
        graph.Import(Application.dataPath + "/" + pbFile);

        //Tensor output = graph.OperationByName("conv2d_1/Relu");
        Tensor input_maps = graph.OperationByName("input_layer");
        Tensor input_weapons = graph.OperationByName("input_1");
        //Tensor output = graph.OperationByName("output_2/BiasAdd");
        Tensor output = graph.OperationByName("output_layer/BiasAdd");

        using (var sess = tf.Session())
        {
            var results = sess.run(output, new FeedItem[]
            {
                new FeedItem(input_maps.outputs[0], maps),
                new FeedItem(input_weapons.outputs[0], weapons)
            });

            Debug.Log(results);
        }


    }
}
