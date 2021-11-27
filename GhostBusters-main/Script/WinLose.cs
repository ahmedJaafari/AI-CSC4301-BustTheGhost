using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinLose : MonoBehaviour 
{
    public UnityEvent buttonClick; 
    public Sprite btn;
   
    public Game var;
    void Awake()
     { 
            if (buttonClick==null) {
                 buttonClick =  new UnityEvent();  
                }
    }
   
     void Start()
     { 
         
         var = FindObjectOfType(typeof(Game)) as Game;
     }

     void Update()
    {
        CheckInput();
        var.CheckInputGrid();
       
    }
   


    private void CheckInput() 
    { 
            if(Input.GetButtonDown("Fire1")) 
            {  

            
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    int a = Mathf.RoundToInt(mousePosition.x);
                    int b = Mathf.RoundToInt(mousePosition.y);
                    if(a>5 || b >14 || a<0 || b<0)
                     {      
              if(var.gx == var.lastcheckedX && var.gy== var.lastcheckedY ) 
               { 

                   Tile ghostcell =  Instantiate(Resources.Load("Prefabs/ghostpic", typeof(Tile)), new Vector3(var.gx, var.gy, 0), Quaternion.identity) as Tile;
                   Tile T = var.grid[var.gx, var.gy];
                   T.SetIsCovered(false);
                   var.grid[var.gx, var.gy]=ghostcell;
                   
                   print("You Busted the ghost ! CONGRATS !");
               } 
                    else print("You busted the wrong cell ! GAME OVER !"); 
                            
                    }
              
            } 
    }


}
