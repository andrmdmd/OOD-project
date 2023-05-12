using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public class Node<T>
    {
        public T value;
        public Node<T>? next;
        public Node<T>? prev;
        public Node(T val, Node<T>? p = null, Node<T>? n = null)
        {
            value = val;
            next = n;
            prev = p;
        }
    }

    public class MyList<T> : ICollection<T>
    {

        Node<T>? head;
        Node<T>? tail;
        int length = 0;

        public MyList()
        {
            head = tail = null;

        }
        public void AddObject(T obj)
        {
            if (head == null)
                head = tail = new Node<T>(obj);
            else
            {
                tail.next = new Node<T>(obj, tail, null);
                tail = tail.next;
            }
            length++;
        }
        public bool DeleteObject(T obj)
        {
            Node<T>? p = head;
            while (p != null)
            {
                if (p.value!.Equals(obj))
                {
                    if (p.Equals(head))
                    {
                        head = head.next;
                        head!.prev = null;
                        length--;
                        return true;
                    }
                    if (p.Equals(tail))
                    {
                        tail = tail.prev;
                        tail!.next = null;
                        length--;
                        return true;
                    }
                    Node<T>? q = p.prev;
                    q.next = p.next;
                    p.next!.prev = q;
                    length--;
                    return true;
                }
            }
            return false;
        }
        public int GetLength()
        {
            return length;
        }

        public IIterator<T> GetForwardIterator()
        {
            return new ListIterator<T>(head);
        }
        public IIterator<T> GetReverseIterator()
        {
            return new RListIterator<T>(tail);
        }

    }
}
