public  class Camera{
  public float X = 0;
  public float Y = 0;
  
  public float dx = 0;
  public float dy = 0;
  
  public float scale = 1;
  
  public float speed = 3;
  
  void Update(){
    if (wPressed){
      dy -= speed;
    }
    if (aPressed){
      dx -= speed;
    }
    
    if (sPressed){
      dy += speed;
    }
    
    if (dPressed){
      dx += speed;
    }
    
    //Update Position
    X += dx;
    Y += dy;
    dx = 0;
    dy = 0;
  }
}


public Boolean pointRectCollision(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2){
  return (
    y1 + h1 > y2 &&
    y1 < y2 + h2 &&
    x1 < x2 + w2 &&
    x1 + w1 > x2
    );
}

public boolean pointRectCollision(int bx, int by, int w, int h, int x, int y){
  if (bx < x && x < bx + w && by < y && y < by + h){
    return true;
  }
  return false;
}

public Boolean mouseClicked = false;
public class MCHandler{
  MCHandler(){
    
  }
  void Update(){
    if (mousePressed){
      mouseClicked = true;
    }
  }
  void PostUpdate(){
    mouseClicked = false;
  }
}


//Keyboard Handling
public Boolean wPressed = false;
public Boolean aPressed = false;
public Boolean sPressed = false;
public Boolean dPressed = false;

public void keyPressed(){
  if (key == 'w'){
    wPressed = true;
  }
  else if (key == 'a'){
    aPressed = true;
  }
  else if (key == 's'){
    sPressed = true;
  }
  else if (key == 'd'){
    dPressed = true;
  }
}

public void keyReleased(){
  if (key == 'w'){
    wPressed = false;
  }
  else if (key == 'a'){
    aPressed = false;
  }
  else if (key == 's'){
    sPressed = false;
  }
  else if (key == 'd'){
    dPressed = false;
  }
}
