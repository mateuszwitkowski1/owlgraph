using System;
using System.IO;
using System.Collections;
using OwlDotNetApi;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace OwlDotNetApiTest
{
	/// <summary>
	/// Summary description for Test.
	/// </summary>
	class Test
	{

        public static Microsoft.Msagl.GraphViewerGdi.GViewer viewer;
        //create a graph object 
        public static Microsoft.Msagl.Drawing.Graph graph2;

        public static Microsoft.Msagl.GraphViewerGdi.GViewer viewer2 = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        //create a graph object 
        public static Microsoft.Msagl.Drawing.Graph graph22 = new Microsoft.Msagl.Drawing.Graph();

        public static System.Windows.Forms.Form form2;

        public static System.Windows.Forms.ComboBox lista = new System.Windows.Forms.ComboBox();

        public static System.Windows.Forms.ComboBox lista2 = new System.Windows.Forms.ComboBox();

        public static IOwlParser parser;
        public static IOwlGraph graph;

        public static List<String> listaOdw = new List<string>();

        public static System.Windows.Forms.Form form22 = new System.Windows.Forms.Form();

        //public static List<Microsoft.Msagl.Drawing.Graph> listagraph = new List<Microsoft.Msagl.Drawing.Graph>();

        public static string wyszukaj(string el1, string el2, string path)
        {
            listaOdw.Add(el1);
            Microsoft.Msagl.Drawing.Node node1 = graph2.FindNode(el1);
            Microsoft.Msagl.Drawing.Node node2 = graph2.FindNode(el2);
            IEnumerator it1 = node1.Edges.GetEnumerator();
            
            while (it1.MoveNext())
            {
                
                Microsoft.Msagl.Drawing.Edge edgec = (Microsoft.Msagl.Drawing.Edge)it1.Current;
                Microsoft.Msagl.Drawing.Node nodetar = edgec.TargetNode;
                Microsoft.Msagl.Drawing.Node nodesou = edgec.SourceNode;
                Microsoft.Msagl.Drawing.Node good;
                if (nodetar.Label.Text == el1) good = nodesou;
                else good = nodetar;
                if (!listaOdw.Contains(good.Label.Text))
                {
                    //listaOdw.Add(good.Label.Text);
                    if (good.Label.Text == el2)
                    {
                        if (nodetar.Label.Text == el1)
                        {
                            Microsoft.Msagl.Drawing.Edge edge = graph22.AddEdge(nodesou.Label.Text, el1);
                            edge.LabelText = edgec.Label.Text;
                        }
                        else
                        {
                            Microsoft.Msagl.Drawing.Edge edge = graph22.AddEdge(el1, nodetar.Label.Text);
                            edge.LabelText = edgec.Label.Text;
                        }
                        graph22.LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings();
                        viewer2.Graph = graph22;
                        //associate the viewer with the form 
                        form22.Size = new Size(1200, 700);
                        form22.SuspendLayout();
                        viewer2.Dock = System.Windows.Forms.DockStyle.Fill;
                        form22.Controls.Add(viewer2);
                        form22.ResumeLayout();
                        //show the form 
                        form22.Show();
                        return path + " " + edgec.Label.Text + " " + good.Label.Text;
                    }
                    else
                    {
                        
                        if (nodetar.Label.Text == el1)
                        {
                            Microsoft.Msagl.Drawing.Edge edge = graph22.AddEdge(nodesou.Label.Text, el1);
                            edge.LabelText = edgec.Label.Text;
                        }
                        else
                        {
                            Microsoft.Msagl.Drawing.Edge edge = graph22.AddEdge(el1, nodetar.Label.Text);
                            edge.LabelText = edgec.Label.Text;
                        }
                        return wyszukaj(good.Label.Text, el2, path + " " + edgec.Label.Text + " " + good.Label.Text);
                    }
                }       
            }
            return "not";
        }

        public void button_click(object sender, EventArgs e)
        {
            lista.Update();
            lista2.Update();

            string el1 = lista.SelectedItem.ToString();
            string el2 = lista2.SelectedItem.ToString();
            listaOdw.Clear();
            graph22 = new Microsoft.Msagl.Drawing.Graph();
            //listaOdw.Add(el1);
            string wynik = wyszukaj(el1, el2, el1);
            Console.WriteLine(wynik);
        }

        public void test9(string file)
        {
            
            parser = new OwlXmlParser();
            graph = parser.ParseOwl(file);

            System.Windows.Forms.Form form = new System.Windows.Forms.Form();

            form.Size = new Size(1200, 700);

            //create a viewer object 
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            graph2 = new Microsoft.Msagl.Drawing.Graph("graph2");

            graph2.LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings();


            form2 = new System.Windows.Forms.Form();

            lista.Size = new Size(200, 30);
            lista.Location = new Point(40, 30);
            lista2.Size = new Size(200, 30);
            lista2.Location = new Point(40, 80);
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.Size = new Size(200, 30);
            button.Location = new Point(40, 130);
            button.Text = "Wybierz";
            button.Click += button_click;

            IDictionaryEnumerator nEnumerator = (IDictionaryEnumerator)graph.Nodes.GetEnumerator();
            while (nEnumerator.MoveNext())
            {
                // Get the node from the graph
                OwlNode node = (OwlNode)graph.Nodes[(nEnumerator.Key).ToString()];
                
                // We will cast the node to a OwlIndividual because we are looking for individuals
                OwlIndividual indNode = node as OwlIndividual;
                // If indNode is different from null, then we are dealing with an OwlIndividual -> OK
                // If the indNode is not anonymous, means that we have an individual with a proper name -> OK
                if ((indNode != null) && (!indNode.IsAnonymous()))
                {
                    // So, now we have a good owl-individual

                    Console.WriteLine(indNode.ID.Replace("urn:absolute:sample#", "") + ":");
                    foreach (OwlEdge a in indNode.ChildEdges)
                    {
                        Console.WriteLine((a.ParentNode.ID + " " + a.ID + " " + a.ChildNode.ID));

                        if (a.ChildNode.ID != "http://www.w3.org/2002/07/owl#NamedIndividual")
                        {
                            Microsoft.Msagl.Drawing.Edge edge = graph2.AddEdge(a.ParentNode.ID, a.ChildNode.ID);
                            edge.LabelText = a.ID;

                            if (!lista.Items.Contains(a.ParentNode.ID))
                            {
                                lista.Items.Add(a.ParentNode.ID);
                                lista2.Items.Add(a.ParentNode.ID);
                            }
                        }
                    }
                    /*
                    foreach (OwlEdge a in indNode.ParentEdges)
                    {
                        Console.WriteLine((a.ParentNode.ID + " " + a.ID + " " + a.ChildNode.ID).Replace("urn:absolute:sample#", "").Replace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", "").Replace("http://www.w3.org/2000/01/rdf-schema#", ""));
                    }
                    */


                    Console.Write("\n \n");
                  
                }
            }
            //bind the graph to the viewer 
            viewer.Graph = graph2;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.Show();
                

            form2.Controls.Add(lista);

            form2.Controls.Add(lista2);

            form2.Controls.Add(button);

            //show the form 
            form2.ShowDialog();


            //Console.WriteLine(graph.Edges.Count);
            Console.ReadLine();

        }


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Test t = new Test();

			// First Example:
			// Reading an ontology from a file and afterwards writing it again to another file
			//t.test1(args[0]); 

			// Second Example:
			// Creating a new ontology with a two classes and making one class a subclass of
			// another class and finally writing it to file
			//t.test2();

			// Third Example:
			// Reading an ontology from a file and retrieving some information from it
			//t.test3("C:\\travel.owl");

			//t.test2("C:\\test.owl");
			//t.test9("C:\\travel.owl");
			t.test9("test.owl");
		}

        public Button button1;
        
    }
}
