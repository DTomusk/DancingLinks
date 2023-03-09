using System;

namespace DancingLinks
{
    /// <summary>
    /// This class contains a toy matrix similar to the full sudoku matrix
    /// It is small enough that the exact state is easy to keep track of 
    /// But large enough to be able to test all the matrix operations in SudokuSolver
    /// </summary>
    public class TestSolver : SudokuSolver
    {
        public void RunTests()
        {
            // Arrange: generic test arrangement
            TestNode testRoot = TestMatrix.setupTestMatrix();
            Node columnToUncover = testCoverColumn(testRoot);
            testUncoverColumn((TestNode)columnToUncover);
        }

        /// <summary>
        /// Test the coverColumn function to ensure that the state of the matrix is as expected after function call
        /// </summary>
        private Node testCoverColumn(TestNode root)
        {
            // Act: cover chosen column
            Node columnToCover = root.East.East;
            coverColumn(columnToCover);

            TestNode node = (TestNode)root.East.East.West;

            // Assert column connectedness:
            if (node.TestLabel != "Column1")
                throw new Exception("Covering column 2 failed to connect column 1 to column 3");

            node = (TestNode)root.East.East;

            if (node.TestLabel != "Column3")
                throw new Exception("Covering column 2 failed to connect column 3 to column 1");

            // Assert node connectedness, removing the second column means the fourth column wraps around
            // and the fifth column points to the second node in its column
            node = (TestNode)root.East.East.East;

            if (node.North != node || node.South != node)
                throw new Exception("Covering column 2 failed to remove the node in column 4");

            if (node.Children != 0)
                throw new Exception("Covering column 2 failed to decrease the number of children in column 4");

            node = (TestNode)root.West;

            if (node.North != node.South)
                throw new Exception("Covering column 2 failed to remove the first node in column 5");

            if (node.Children != 1)
                throw new Exception("Covering column 2 failed to decrease the number of children in column 5");

            // Assert that the columns expected to be unaffected haven't been affected
            node = (TestNode)root.East;

            if (node.Children != 2)
                throw new Exception("Covering column 2 decreased the number of children in column 1");

            node = (TestNode)root.East.East;

            if (node.Children != 2)
                throw new Exception("Covering column 2 decreased the number of children in column 3");

            Console.WriteLine("Cover column tests succeeded");
            return columnToCover;
        }

        /// <summary>
        /// Test the uncoverColumn function to ensure that the state of the matrix is as expected after function call
        /// </summary>
        private void testUncoverColumn(TestNode columnToUncover)
        {
            // Act: uncover the column that was covered in the earlier test 
            uncoverColumn(columnToUncover);

            // Assert: check column siblings have been updated
            if (columnToUncover.East.West != columnToUncover)
                throw new Exception("Column 2's east neighbour not updated correctly after being uncovered");
            if (columnToUncover.West.East != columnToUncover)
                throw new Exception("Column 2's west neighbour not updated correctly after being uncovered");

            // Assert: check the number of children is back to as it was
            if (columnToUncover.West.Children != 2)
                throw new Exception("Column 1's children not as expected after uncovering");
            if (columnToUncover.Children != 2)
                throw new Exception("Column 2's children not as expected after uncovering");
            if (columnToUncover.East.Children != 2)
                throw new Exception("Column 3's children not as expected after uncovering");
            if (columnToUncover.East.East.Children != 1)
                throw new Exception("Column 4's children not as expected after uncovering");
            if (columnToUncover.East.East.East.Children != 2)
                throw new Exception("Column 5's children not as expected after uncovering");

            // Assert: check column 4 and 5 have the correct children
            TestNode node = (TestNode)columnToUncover.East.East.South;

            if (node.TestLabel != "Node1Column4")
                throw new Exception($"Column 4's child not added back correctly, label was {node.TestLabel}");

            node = (TestNode)columnToUncover.East.East.East.South;

            if (node.TestLabel != "Node1Column5")
                throw new Exception($"Column 5's child not added back correctly, label was {node.TestLabel}");

            Console.WriteLine("Uncover column tests succeeded");
        }
    }

    /// <summary>
    /// The TestMatrix is a simpler version of the matrix produced by SudokuGenerator 
    /// As a matrix of 1s and 0s, the nodes appear as: 
    /// 1 0 1 0 0 
    /// 0 1 0 1 0
    /// 0 1 0 0 1
    /// 1 0 1 0 1
    /// This has only one solution, namely rows 2 and 4
    /// </summary>
    public class TestMatrix
    {
        public static TestNode setupTestMatrix()
        {
            TestMatrix testMatrix = new TestMatrix();

            // Create test column headers 
            TestNode root = new TestNode("Root");
            TestNode col1 = new TestNode("Column1") { Children = 2 };
            TestNode col2 = new TestNode("Column2") { Children = 2 };
            TestNode col3 = new TestNode("Column3") { Children = 2 };
            TestNode col4 = new TestNode("Column4") { Children = 1 };
            TestNode col5 = new TestNode("Column5") { Children = 2 };

            // Create test nodes
            TestNode node1col1 = new TestNode("Node1Column1");
            TestNode node2col1 = new TestNode("Node2Column1");
            TestNode node1col2 = new TestNode("Node1Column2");
            TestNode node2col2 = new TestNode("Node2Column2");
            TestNode node1col3 = new TestNode("Node1Column3");
            TestNode node2col3 = new TestNode("Node2Column3");
            TestNode node1col4 = new TestNode("Node1Column4");
            TestNode node1col5 = new TestNode("Node1Column5");
            TestNode node2col5 = new TestNode("Node2Column5");

            // Connect root
            testMatrix.connectNodes(root, root, col1, root, col5, root);

            // Connect column headers to their adjacent nodes
            testMatrix.connectNodes(col1, node2col1, col2, node1col1, root, col1);
            testMatrix.connectNodes(col2, node2col2, col3, node1col2, col1, col2);
            testMatrix.connectNodes(col3, node2col3, col4, node1col3, col2, col3);
            testMatrix.connectNodes(col4, node1col4, col5, node1col4, col3, col4);
            testMatrix.connectNodes(col5, node2col5, root, node1col5, col4, col5);

            // Connect all nodes to their adjacent nodes 
            // Row 1:
            testMatrix.connectNodes(node1col1, col1, node1col3, node2col1, node1col3, col1);
            testMatrix.connectNodes(node1col3, col3, node1col1, node2col3, node1col1, col3);

            // Row 2: 
            testMatrix.connectNodes(node1col2, col2, node1col4, node2col2, node1col4, col2);
            testMatrix.connectNodes(node1col4, col4, node1col2, col4, node1col2, col4);

            // Row 3: 
            testMatrix.connectNodes(node2col2, node1col2, node1col5, col2, node1col5, col2);
            testMatrix.connectNodes(node1col5, col5, node2col2, node2col5, node2col2, col5);

            // Row 4: 
            testMatrix.connectNodes(node2col1, node1col1, node2col3, col1, node2col5, col1);
            testMatrix.connectNodes(node2col3, node1col3, node2col5, col3, node2col1, col3);
            testMatrix.connectNodes(node2col5, node1col5, node2col1, col5, node2col3, col5);

            return root;
        }

        private void connectNodes(TestNode center, TestNode north, TestNode east, TestNode south, TestNode west, TestNode header)
        {
            center.North = north;
            center.East = east;
            center.South = south;
            center.West = west;
            center.Header = header;
        }
    }

    public class TestNode: Node
    {
        public string TestLabel { get; set; }

        public TestNode(string label) : base()
        {
            TestLabel = label;
        }
    }
}
