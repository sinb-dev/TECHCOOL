using System;
using System.Collections.Generic;

namespace TECHCOOL 
{
    public abstract class Component 
    {
        public abstract void Render(int x, int y, int width, int height);
        protected List<Component> children { get; private set; }= new List<Component>();
        public RenderInfo RenderInfo {get;set;}
        public void Add(Component c) {
            children.Add(c);
        }
    }
    public abstract class Field : Component
    {
        
    }
    public class RenderInfo {
        public int X { get;set;}
        public int Y { get;set;}
        public int Width { get;set;}
        public int Height { get;set;}

        public int CursorX { get;set;}
        public int CursorY {get;set;}
        public RenderInfo(int x, int y) {
            X = x;
            Y = y;
        }
        public RenderInfo(int x, int y, int width, int height)  
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public RenderInfo(int x, int y, int width, int height, int cursorX, int cursorY)  
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            CursorX = cursorX;
            CursorY = cursorY;
        }
    }
}