//0 = background objects
//1 = entities
//2 = textures
//3 = foreground objects
public int layer = 1;

public final int tileSize = 50;

PrintWriter output;

//Inventory
public String[] inventory = {
  "platform",
  "player",
  "goal",
  "arrow1",
  "arrow2",
  "arrow3"
};

public String item = inventory[0];
public Boolean inInventory = false;

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
    createMap();
  }
  
  //Main
  void Update(){
    if (!inInventory){
      //Create Tiles
      if (pointRectCollision(0, 0, width, height, mouseX, mouseY)){
        //Goes through grid and finds one that mouse is over
        for (int y = 0; y < map.mapSize.Y; y++){
          for (int x = 0; x < map.mapSize.X; x++){
            if (pointRectCollision(int(x * map.tileSize - camera.X), int(y * map.tileSize - camera.Y), map.tileSize, map.tileSize, mouseX, mouseY)){
              
              //Actually creates entity
              if (mousePressed && mouseButton == LEFT){
                Boolean oneFound = false;
                for (int i = 0; i < EntityHandler.entities.size(); i++){
                  if (pointRectCollision(int(EntityHandler.entities.get(i).X - camera.X), 
                  int(EntityHandler.entities.get(i).Y - camera.Y), 
                  int(EntityHandler.entities.get(i).Width), 
                  int(EntityHandler.entities.get(i).Height), 
                  mouseX, mouseY)){
                    oneFound = true;
                  }
                }
                if (!oneFound) EntityHandler.entities.add(new Entity(item, x * map.tileSize, y * map.tileSize));
              } 
            }
  
          }
        }
        
        //Remove Entity
        if (mousePressed && mouseButton == RIGHT){
          for (int i = 0; i < EntityHandler.entities.size(); i++){
            Entity entity = EntityHandler.entities.get(i);
            if (pointRectCollision(int(entity.X - camera.X), int(entity.Y - camera.Y), int(entity.Width), int(entity.Height), mouseX, mouseY)){
              EntityHandler.entities.remove(i);
            }
          }
        }
      }
    }
    
    else{
      for (int i = 0; i < inventory.length; i++){
        fill(175, 175, 175);
        rect(30 + 150 * i, 200, 100, 30);
        fill(0, 0, 0);
        text(inventory[i], 30+150 * i, 200+6);
        
        if (pointRectCollision(30 + 150 * i, 200, 100, 30, mouseX, mouseY)){
          if (mousePressed && mouseButton == LEFT){
            item = inventory[i];
          }
        }
      }
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
    
    text("Item Selected: " + item, 0, 32+16);
  }
  
  void exportMap(){
    optimizeMap();
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
  
  void optimizeMap(){
    //Gets rid of entities with same x y with and height
    for (int i = 0; i < EntityHandler.entities.size(); i++){
      for (int j = 0; j < EntityHandler.entities.size(); j++){
        if (EntityHandler.entities.get(i) == EntityHandler.entities.get(j)) continue;
        if (EntityHandler.entities.get(i).X == EntityHandler.entities.get(j).X &&
        EntityHandler.entities.get(i).Y == EntityHandler.entities.get(j).Y &&
        EntityHandler.entities.get(i).Width == EntityHandler.entities.get(j).Width &&
        EntityHandler.entities.get(i).Height == EntityHandler.entities.get(j).Height){
          EntityHandler.entities.remove(i);
        }
      }
    }
    
    //Optimizes along x axis
    Boolean found = false;
    while (true){
      found = true;
      
      for (int i = 0; i < EntityHandler.entities.size(); i++){
        Entity entity = EntityHandler.entities.get(i);
        
        for (int j = 0; j < EntityHandler.entities.size(); j++){
          if (entity == EntityHandler.entities.get(j)) continue;
          
          if (entity.X + entity.Width == EntityHandler.entities.get(j).X && entity.Y == EntityHandler.entities.get(j).Y){
            entity.Width += EntityHandler.entities.get(j).Width;
            EntityHandler.entities.remove(j);
            found = false;
          }
        }
      }
      if (found) break;
    }
    
    //Optimizes along y axis
    
    found = false;
    while (true){
      found = true;
      
      for (int i = 0; i < EntityHandler.entities.size(); i++){
        Entity entity = EntityHandler.entities.get(i);
        
        for (int j = 0; j < EntityHandler.entities.size(); j++){
          if (entity == EntityHandler.entities.get(j)) continue;
          
          if (entity.Y + entity.Height == EntityHandler.entities.get(j).Y && entity.X == EntityHandler.entities.get(j).X && entity.Width == EntityHandler.entities.get(j).Width){
            entity.Height += EntityHandler.entities.get(j).Height;
            EntityHandler.entities.remove(j);
            found = false;
          }
        }
      }
      if (found) break;
    }
    
  }
  
  void createMap(){
    String[] rawMap = loadStrings("map.txt");
    
    for (int i = 0; i < split(split(rawMap[0], ':')[1], ';').length-1; i++){
      String[] rawData = split(split(split(rawMap[0], ':')[1], ';')[i], ',');
      
      println("Done: " + i);
      println("Map Data = " + rawData[0] + ", " + rawData[1] + ", " + rawData[2] + ", " + rawData[3] + ", " + rawData[4]);
      println("");
      EntityHandler.entities.add(new Entity(rawData[0], float(rawData[1]), float(rawData[2]), float(rawData[3]), float(rawData[4])));
      
    }
    
    /*
                for (int i = 0; i < rawMapData.Split(':')[1].Split(';').Length; i++)
            {
                String[] rawData = rawMapData.Split(':')[1].Split(';')[i].Split(',');

                switch (rawData[0])
                {
                    case "player":
                        EntityHandler.entities.Add(new Player(float.Parse(rawData[1]), float.Parse(rawData[2])));
                        //Console.WriteLine("Player Created");
                        break;
                    case "platform":
                        EntityHandler.entities.Add(new Platform(
                            float.Parse(rawData[1]),
                            float.Parse(rawData[2]),
                            float.Parse(rawData[3]),
                            float.Parse(rawData[4])));
                        //Console.WriteLine("Platform Created");
                        break;
                    case "goal":
                        goalHandler.createGoal(float.Parse(rawData[1]), float.Parse(rawData[2]));
                        //Console.WriteLine("Goal Created");
                        break;
                }
            }
    */
  }
}
