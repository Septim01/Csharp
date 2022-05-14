using System.Collections.Generic;
using System.IO;
namespace Scratch {
	public static class MyMath {
		public static int Gcd(int a, int b) {
			while (b != 0) {
				var t = a % b;
				a = b;
				b = t;
			}

			return a;
		}

		public static int Lcm(int a, int b) {
			return a * b * Gcd(a, b);
		}
	}

	internal abstract class Entry {
		public enum T {
			Rn,
			Op
		}

		public T Type;

		public abstract string GetOp();
		public abstract RationalNumber GetRn();
		public abstract void SetRn(int numerator, int denominator);
	}

	internal class RationalNumber : Entry {
		public int N, D;

		public override string GetOp() {
			throw new System.NotImplementedException();
		}

		public override RationalNumber GetRn() {
			return this;
		}

		public override void SetRn(int numerator, int denominator) {
			var g = MyMath.Gcd(numerator, denominator);
			N = numerator / g;
			D = denominator / g;
		}

		public RationalNumber(int numerator, int denominator) {
			Type = T.Rn;
			N = numerator;
			D = denominator;
		}
	}

	internal class Operation : Entry {
		private readonly string _op;

		public override string GetOp() {
			return _op;
		}

		public override RationalNumber GetRn() {
			throw new System.NotImplementedException();
		}

		public override void SetRn(int numerator, int denominator) {
			throw new System.NotImplementedException();
		}

		public Operation(string operation) {
			Type = T.Op;
			_op = operation;
		}
	}

	internal static class Program {
		private static string GetRnString(RationalNumber a) {
			// ReSharper disable once StringLiteralTypo
			if (a.D == 0) return "nelze";
			if (a.D == 1) return a.N.ToString();
			if (a.D < 0) {
				a.D *= -1;
				a.N *= -1;
			}

			return a.N + "/" + a.D;
		}

		private static string GetBufferString(ref List<Entry> buffer) {
			List<string> arr = new();
			foreach (var x in buffer) {
				if (x.Type == Entry.T.Rn) {
					arr.Add(GetRnString(x.GetRn()));
				}

				if (x.Type == Entry.T.Op) {
					arr.Add(x.GetOp());
				}
			}

			return string.Join(' ', arr);
		}

		private static string ParseLine(string line) {
			RationalNumber leftSide = new(0, 0);
			var sideOp = '\0';
			List<Entry> buffer = new();

			var i = 0;
			while (i < line.Length) {
				var nothingDone = true;

				if (line.Length >= 6 + i && line.StartsWith("int(")) {
					buffer.Add(new Operation("int"));
					i += 4;
					nothingDone = false;
				}

				if (i != line.Length &&
				    line[i] == ')' &&
				    buffer.Count >= 2) {
					if (buffer[^2].GetOp() == "int") {
						buffer[^2] = buffer[^1];
						buffer.RemoveAt(buffer.Count - 1);
						var a = buffer[^1].GetRn();
						var newNumerator = 0;
						while (a.N >= a.D) {
							newNumerator++;
							a.N -= a.D;
						}

						a.N = newNumerator;
						a.D = 1;
					}

					i++;
					nothingDone = false;
				}

				if (i != line.Length && "><=".Contains(line[i])) {
					leftSide = buffer[0].GetRn();
					sideOp = line[i];
					buffer.RemoveRange(0, buffer.Count);
					i++;
					nothingDone = false;
				}

				if (i != line.Length && "+-*\\".Contains(line[i])) {
					buffer.Add(new Operation(char.ToString(line[i])));
					i++;
					nothingDone = false;
				}

				if (i != line.Length && char.IsNumber(line[i])) {
					RationalNumber number = new(0, 1);
					while (i < line.Length && char.IsNumber(line[i])) {
						number.N = number.N * 10 + line[i] - '0';
						i++;
					}

					if (i < line.Length - 1 && line[i] == '/') {
						i++;
						number.D = 0;
						while (i != line.Length && i < line.Length && char.IsNumber(line[i])) {
							number.D = number.D * 10 + line[i] - '0';
							i++;
						}

						// ReSharper disable once StringLiteralTypo
						if (number.D == 0) return "nelze";
					}

					var d = MyMath.Gcd(number.N, number.D);
					number.N /= d;
					number.D /= d;

					buffer.Add(number);
					nothingDone = false;
				}

				if (buffer.Count >= 3 &&
				    buffer[^3].Type == Entry.T.Rn &&
				    buffer[^2].Type == Entry.T.Op &&
				    buffer[^1].Type == Entry.T.Rn) {
					var a = buffer[^3].GetRn();
					var b = buffer[^1].GetRn();
					var m = MyMath.Lcm(a.D, b.D);
					if (buffer[^2].GetOp() == "+") {
						buffer[^3].SetRn(a.N * m / a.D + b.N * m / b.D, m);
					}

					if (buffer[^2].GetOp() == "-") {
						buffer[^3].SetRn(a.N * m / a.D - b.N * m / b.D, m);
					}

					if (buffer[^2].GetOp() == "*") {
						buffer[^3].SetRn(a.N * b.N, a.D * b.D);
					}

					if (buffer[^2].GetOp() == "\\") {
						buffer[^3].SetRn(a.N * b.D, a.D * b.N);
					}

					buffer.RemoveRange(buffer.Count - 2, 2);
					nothingDone = false;
				}

				if (nothingDone) i++;
			}

			if (sideOp != '\0') {
				var a = leftSide;
				var b = buffer[0].GetRn();
				var m = MyMath.Lcm(a.D, b.D);
				if (sideOp == '>') return m * a.N / a.D > m * b.N / b.D ? "true" : "false";
				if (sideOp == '<') return m * a.N / a.D < m * b.N / b.D ? "true" : "false";
				if (sideOp == '=') return a.N * b.D == b.N * a.D ? "true" : "false";
			}

			return GetBufferString(ref buffer);
		}

		private static void Main() {
			// ReSharper disable once StringLiteralTypo
			var lines = File.ReadAllLines("vstup.txt");
			var outLines = new string[lines.Length];

			for (int i = 0; i < lines.Length; i++) {
				outLines[i] = ParseLine(lines[i]);
			}

			// ReSharper disable once StringLiteralTypo
			File.WriteAllLines("vystup.txt", outLines);
		}
	}
}
