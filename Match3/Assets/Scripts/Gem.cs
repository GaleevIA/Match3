using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour 
{
    private bool selected = false;
    private Dictionary<string, int> coordinate;

    public static GameObject Start(GameObject gem, Vector2 position, int x, int y, float tileSize, Transform parent)
    {
        return Instantiate(gem, new Vector2(position.x + (x * tileSize), position.y + (y * tileSize)), Quaternion.identity, parent);
    }

    //Нажатие мышкой на текущую ячейку
    void OnMouseDown()
    {
        //Выбор первой ячейки
        if (!selected && !GameController.instance.isSelected)
        {
            selected = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            GameController.instance.SetIsSelected(selected);
            GameController.instance.SetSelectedCoordinates(true, coordinate["x"], coordinate["y"]);      
        }
        //Выбор второй ячейки
        else if (!selected && GameController.instance.isSelected)
        {
            GameController.instance.SetSelectedCoordinates(false, coordinate["x"], coordinate["y"]);
            GameController.instance.CheckIfPossibleMove();        
        }
        //Отмена выбора первой ячейки
        else if (selected)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            selected = false;
            GameController.instance.SetIsSelected(selected);
        }
    }

    //Отменить выбранную ячейку
    public void ResetSelection()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        selected = false;
    }

    //Задать координаты ячейки
    public void SetCoordinates(Dictionary<string,int> dic)
    {
        coordinate = dic;
    }
}