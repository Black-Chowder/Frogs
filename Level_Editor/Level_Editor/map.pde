//0 = background objects
//1 = entities
//2 = textures
//3 = foreground objects
public int layer = 1;

public String item = "player";

public final int tileSize = 50;

PrintWriter output;

//Map Arrays


public class Map{
  public int tileSize = 50;
  
  public String[] rawMap1;

  public Vector2 mapSize = new Vector2(100, 100);
  
  public int[][] backgroundObjects = new int[int(mapSize.Y)][int(mapSize.X)];
  
  public int[][] entityArray = new int[int(mapSize.Y)][int(mapSize.X)];
  
  public int[][] textureArray = new int[int(mapSize.Y)][int(mapSize.X)];
  
  public int[][] foregroundObjects = new int[int(mapSize.Y)][int(mapSize.X)];
  
}

//Map Handler Class
public class MapHandler{
  //Variables
  public Map map;
  
  //Constructor
  MapHandler(){
    map = new Map();
  }
  
  //Main
  void Update(){
    //Create Tiles
    if (pointRectCollision(0, 0, width, height, mouseX, mouseY)){
      for (int y = 0; y < map.mapSize.Y; y++){
        for (int x = 0; x < map.mapSize.X; x++){
          if (pointRectCollision(int(x * map.tileSize - camera.X), int(y * map.tileSize - camera.Y), map.tileSize, map.tileSize, mouseX, mouseY)){
            
            
            if (mousePressed && mouseButton == LEFT){
              EntityHandler.entities.add(new Entity(item, x * map.tileSize, y * map.tileSize));
            }            
          }

        }
      }
      
      //EntityHandler.entities.add(new Entity("player", mouseX, 0, 50, 50));
    }
  }
  
  //Drawing
  void Draw(){
    //Display Grid
    fill(0, 0, 0);
    for (int i = 0; i < map.mapSize.X; i++){
      stroke(126);
      line(i * map.tileSize - camera.X, 0, i * map.tileSize - camera.X, height);
    }
    
    for (int i = 0; i < map.mapSize.Y-1; i++){
      stroke(126);
      line(0, i * map.tileSize - camera.Y, width, i * map.tileSize - camera.Y);
    }
  }
  
  void exportMap(){
    output = createWriter("map.txt");
    
    //backgroundObjects
    //entityArrays
    //textureArrays
    //foregroundObjects
    
    
    output.print("Level1");
    output.print(":");
    for (int i = 0; i < EntityHandler.entities.size(); i++){
      Entity entity = EntityHandler.entities.get(i);
      
      output.print(entity.classId + "," + entity.X + "," + entity.Y + "," + entity.Width + "," + entity.Height + ";");
      
    }
    
    //Buffer
    output.print("\n\n\n\n\n\n\n\n\n\n\n\n");
    for (int i = 0; i < 50000; i++){
      output.print("yee" + str(i));
    }
    
    println("EXPORT COMPLETE");
  }
}
