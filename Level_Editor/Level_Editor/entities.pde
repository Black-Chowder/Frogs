static class EntityHandler{
  public static ArrayList<Entity> entities = new ArrayList<Entity>();
  
  static void Update(){
    for (int i = 0; i < entities.size(); i++){
      entities.get(i).Update();
    }
  }
  
  static void Draw(){
    for (int i = 0; i < entities.size(); i++){
      entities.get(i).Draw();
    }
  }
}

class Entity{
  public String classId;
  public float X;
  public float Y;
  public float Width;
  public float Height;
  
  Entity(String classId, float X, float Y){
    this.classId = classId;
    this.X = X;
    this.Y = Y;
    this.Width = tileSize;
    this.Height = tileSize;
  }
  
  Entity(String classId, float X, float Y, float Width, float Height){
    this.classId = classId;
    this.X = X;
    this.Y = Y;
    this.Width = Width;
    this.Height = Height;
  }
  
  void Update(){
    
  }
  
  void Draw(){
    fill(255, 0, 0);
    rect(this.X - camera.X, this.Y - camera.Y, this.Width, this.Height);
  }
}
