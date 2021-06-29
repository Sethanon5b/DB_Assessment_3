using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BinaryTree 
{
    // Nested Class Implementation
    public BinaryTreeNode root;
    public class BinaryTreeNode 
    {
        public int index;
        public string username;
        public int score;
        public BinaryTreeNode left;
        public BinaryTreeNode right;
    }

    // Constructor
    public BinaryTree() { }

    public BinaryTree(BinaryTreeNode _root) 
    {
        root = _root;
    }

    /// <summary>
    /// This creates the node at the index specified and specifies it's neighbour nodes
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CreateNodeAtIndex(int index) 
    {
        BinaryTreeNode before = null;
        BinaryTreeNode after = root;

        while(after != null) 
        {
            before = after;
            if(index < after.index) 
            {
                after = after.left;
            }
            else if(index > after.index) 
            {
                after = after.right;
            }
            else 
            {
                return false;
            }
        }

        BinaryTreeNode newNode = new BinaryTreeNode();
        newNode.index = index;

        if(root == null) 
        {
            root = newNode;
        }
        else 
        {
            if(index < before.index) 
            {
                before.left = newNode;
            }
            else 
            {
                before.right = newNode;
            }
        }
        return true;
    }

    /// <summary>
    /// Creates a new node within the binary tree at the next logical index
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool CreateNode(BinaryTreeNode node) 
    {
        if(node != null) 
        {
            if(Find(node.index) == null) 
            {
                BinaryTreeNode before = null;
                BinaryTreeNode after = root;

                while(after != null) 
                {
                    before = after;
                    if(node.index < after.index) 
                    {
                        after = after.left;
                    }
                    else if(node.index > after.index) 
                    {
                        after = after.right;
                    }
                    else 
                    {
                        return false;
                    }
                }

                if(root == null) 
                {
                    root = node;
                }
                else 
                {
                    if(node.index < before.index) 
                    {
                        before.left = node;
                    }
                    else 
                    {
                        before.right = node;
                    }
                }
                return true;
            }
        }
        return false;
    }

    #region Comparator Implementation // Using this to find Records on the highscore scene
    /// <summary>
    /// This is an override for the Find function which assumes the root is the default search start position
    /// </summary>
    /// <param name="index">The index to look for within the binary tree</param>
    /// <returns>The binary tree node at the relevant index</returns>
    public BinaryTreeNode Find(int index)
    {
        return Find(index, root);
    }

    /// <summary>
    /// Takes the index and input parent node (the place to start looking from) and recursively searches through the binary tree and looks for the input index.
    /// </summary>
    /// <param name="index">The index to look for within the binary tree.</param>
    /// <param name="parent">The node at which to commence searching through the binary tree.</param>
    /// <returns></returns>
    private BinaryTreeNode Find(int index, BinaryTreeNode parent) 
    {
        if(parent != null) 
        {
            if(index == parent.index) 
            {
                return parent;
            }
            else if(index < parent.index) 
            {
                return Find(index, parent.left);
            }
            else 
            {
                return Find(index, parent.right);
            }
        }
        return null;
    }
    #endregion 

}
