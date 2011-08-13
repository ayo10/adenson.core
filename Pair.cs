using System;
using System.ComponentModel;

namespace Adenson.ListenQuest
{
	public struct Pair<T> : IEquatable<Pair<T>>, INotifyPropertyChanged
	{
		#region Variables
		private T _left;
		private T _right;
		#endregion
		#region Constructor

		public Pair(T index, T total) : this()
		{
			this.Left = index;
			this.Right = total;
		}

		#endregion
		#region Properties

		public T Left
		{
			get { return _left; }
			set 
			{
				if (!Object.Equals(_left, value))
				{
					_left = value;
					this.OnPropertyChanged("Left");
				}
			}
		}
		public T Right
		{
			get { return _right; }
			set
			{
				if (!Object.Equals(_right, value))
				{
					_right = value;
					this.OnPropertyChanged("Right");
				}
			}
		}
		public bool IsEmpty
		{
			get { return this.Equals(new Pair<T>()); }
		}
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
		#region Methods

		public bool Equals(Pair<T> other)
		{
			if (Object.ReferenceEquals(this, other)) return true;
			if (Object.ReferenceEquals(other, null)) return false;
			return Object.Equals(this.Left, other.Left) && Object.Equals(this.Right, other.Right);
		}
		public override bool Equals(object obj)
		{
			if (Object.ReferenceEquals(this, obj)) return true;
			if (obj is Pair<T>) return this.Equals((Pair<T>)obj);
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}
		public Pair<T> Fill(Pair<T> part)
		{
			if (this.IsEmpty) return part;
			if (this.Left != null && this.Right != null) return this;
			else if (this.Left != null) return new Pair<T>(this.Left, part.Right);
			else if (this.Right != null) return new Pair<T>(part.Left, this.Right);
			return this;
		}
		public override string ToString()
		{
			if (this.IsEmpty) return String.Empty;
			else if (this.Right == null) return this.Left == null ? String.Empty : this.Left.ToString();
			else return String.Concat(this.Left, ", ", this.Right);
		}
		private void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
		#region Operators

		public static bool operator ==(Pair<T> pair1, Pair<T> pair2)
		{
			return pair1.Equals(pair2);
		}
		public static bool operator !=(Pair<T> pair1, Pair<T> pair2)
		{
			return !pair1.Equals(pair2);
		}

		#endregion
	}
}