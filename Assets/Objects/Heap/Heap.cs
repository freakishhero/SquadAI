using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T> {

    T[] items;

    int current_item_count;

    public Heap(int _max_size)
    {
        items = new T[_max_size];
    }

    public void Add(T _item)
    {
        _item.Heap_Index = current_item_count;
        items[current_item_count] = _item;
        SortUp(_item);
        current_item_count++;
    }

    public T RemoveFirst()
    {
        T first_item = items[0];
        current_item_count--;

        items[0] = items[current_item_count];
        items[0].Heap_Index = 0;
        SortDown(items[0]);
        return first_item;
    }

    void SortDown(T _item)
    {
        while(true)
        {
            int left_child_index = _item.Heap_Index * 2 + 1;
            int right_child_index = _item.Heap_Index * 2 + 2;
            int swap_index = 0;

            if (left_child_index < current_item_count)
            {
                swap_index = left_child_index;

                if(right_child_index < current_item_count)
                {
                    if(items[left_child_index].CompareTo(items[right_child_index]) < 0)
                    {
                        swap_index = right_child_index;
                    }
                }

                if(_item.CompareTo(items[swap_index]) < 0)
                {
                    Swap(_item, items[swap_index]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            } 
        }
    }

    public void UpdateItem(T _item)
    {
        SortUp(_item);
    }

    public int Count
    {
        get
        {
            return current_item_count;
        }
    }

    public bool Contains(T _item)
    {
        return Equals(items[_item.Heap_Index], _item);
    }

    void SortUp(T _item)
    {
        int parent_index = (_item.Heap_Index - 1) / 2;

        while(true)
        {
            T parent_item = items[parent_index];
            if(_item.CompareTo(parent_item) > 0)
            {
                Swap(_item, parent_item);
            }
            else
            {
                break;
            }

            parent_index = (_item.Heap_Index - 1) / 2;
        }
    }

    void Swap(T _item_1, T _item_2)
    {
        items[_item_1.Heap_Index] = _item_2;
        items[_item_2.Heap_Index] = _item_1;

        int item_1_index = _item_1.Heap_Index;
        _item_1.Heap_Index = _item_2.Heap_Index;
        _item_2.Heap_Index = item_1_index;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int Heap_Index { get; set; }
}
