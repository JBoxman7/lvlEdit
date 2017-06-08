using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    private Wall[] walls;
    private bool alive = false;
    GeometryController parentMap;

    private void Start() {
        parentMap = transform.parent.GetComponentInParent<GeometryController>();
        walls = new Wall[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            Wall newWall = new Wall(transform.GetChild(i));
            walls[i] = newWall;
        }
        CheckNeighbors();
    }

    public void CheckNeighbors() {

        Block[] neighbors = parentMap.GetNeighbors(this);
        for (int c = 0; c < neighbors.Length; c++) {
            Block neighbor = neighbors[c];
            bool flag = true;
            if (neighbor != null) {
                flag = (alive && !neighbor.alive) ? true : false;
            } else if (parentMap.IsEdgeBlock(this)) {
                flag = (alive) ? true : false;
            }
            switch (flag) {
                case true:
                    walls[c].Show();
                    break;
                case false:
                    walls[c].Hide();
                    break;
            }
        }
    }

    public void UpdatePosition() {
        transform.position = parentMap.CoordToPosition(parentMap.GetBlockCoord(this));
    }

    public bool IsAlive() { return alive; }

    public void Kill() { alive = false; }

    public void Invert() { alive ^= true; }

    private struct Wall {
        private Transform wall;
        private Renderer rend;
        public string name;

        public Wall(Transform _wall) {
            wall = _wall;
            rend = wall.GetComponent<Renderer>();
            name = wall.name;
        }

        public void Hide() {
            wall.GetComponent<Renderer>().enabled = false;
        }

        public void Show() {
            wall.GetComponent<Renderer>().enabled = true;
        }
    }
}
