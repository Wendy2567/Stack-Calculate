using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnGiaiThuat2
{
   
    public class SupportTools
    {
        // Hàm xét độ ưu tiên của phép toán
        public int GetPriority(string value)
        {
            if (value == "+" || value == "-") return 1;
            else if (value == "*" || value == "/") return 2;
            else return -1;
        }

        // Hàm kiểm tra kí tự ngoặc đơn
        public bool isParentheses(string value)
        {
            if (value == "(" || value == ")") return true;
            else return false;
        }

        // Hàm kiểm tra phép toán
        public bool isOperator(string value)
        {
            if (value != "+" && value != "-" && value != "*" && value != "/" && value != "(" && value != ")")
                return false;
            else return true;
        }
    }
    public class Infix
    {
        private SupportTools tool = new SupportTools();
        private List<string> lst_infix = new List<string>();
        //Hàm đơn giản hóa input người dùng
        private string ChangeToInfixSys(string input)
        {
            string infixUI = input;
            string infixSys = "";
            Stack<char> contain = new Stack<char>();
            bool isError = false;
            foreach (char v in infixUI)
            {
                if ((tool.isOperator(v.ToString()) || v == '.') && !tool.isParentheses(v.ToString()))
                {
                    contain.Push(v);
                    continue;
                }
                else if (contain.Count >= 1 && (Char.IsDigit(v) || tool.isParentheses(v.ToString())))
                {
                    while (contain.Count > 1)
                    {
                        char? first = contain.Pop();
                        char? second = contain.Pop();
                        if ((first == '-' && second == '-') || (first == '+' && second == '+')) contain.Push('+');
                        else if ((first == '-' && second == '+') || (first == '+' && second == '-')) contain.Push('-');
                        else
                        {
                            isError = true;
                            break;
                        }
                    }
                    if (isError) { contain.Clear(); infixSys = ""; break; }
                    else
                    {
                        if (contain.Peek() == '+' && (infixSys.Length == 0 || infixSys[infixSys.Length - 1] == '(')) { contain.Pop(); }
                        else infixSys += contain.Pop();
                    }
                }
                else if (infixSys.Length > 0 && contain.Count == 0 && Char.IsDigit(infixSys[infixSys.Length - 1]) && v == '(')
                    infixSys += "*";
                infixSys += v;
            }
            if ((infixSys.Length <= 0 || infixSys == null) && isError) return infixSys = "Error";
            else return infixSys;
        }

        // Hàm tạo danh sách infix
        public List<string> CreateLstInfix(string input)
        {
            string infix = ChangeToInfixSys(input);
            char? temp = null;
            string num = "";
            while (infix.Length > 0)
            {
                if (!Char.IsDigit(infix[0]))
                {
                    temp = infix.ElementAt(0);
                    infix = infix.Remove(0, 1);

                    if (num.Length > 0 && temp != '.')
                    {
                        lst_infix.Add(num);
                        num = "";
                    }

                    if (((lst_infix.Count == 0
                        || (lst_infix.Count > 0 && lst_infix[lst_infix.Count - 1] == "("))
                        && temp == '-') || temp == '.')
                        continue;
                    else
                    {
                        lst_infix.Add(temp.ToString());
                        temp = null;
                    }
                }
                else
                {
                    if (temp != null)
                    {
                        num += temp;
                        temp = null;
                    }
                    num += infix[0].ToString();
                    infix = infix.Remove(0, 1);
                }
            }
            if (num != "") lst_infix.Add(num);
            return lst_infix;
        }
    }
    public class Postfix : Infix
    {
        private SupportTools tool = new SupportTools();
        public string input;
        protected List<string> lst_infix = new List<string>();
        protected List<string> lst_postfix = new List<string>();

        // Hàm chuyển lấy postfix
        public List<string> GetPostfixLst()
        {
            this.lst_postfix = ChangeToPostFix(this.lst_postfix);
            return lst_postfix;
        }

        // Hàm chuyển Lấy InfixLst

        private void GetInfixLst()
        {
            lst_infix = base.CreateLstInfix(input);
        }

        // Hàm chuyển Infix sang PostFix
        private List<string> ChangeToPostFix(List<string> lst_postfix)
        {
            GetInfixLst();
            Stack<string> stack = new Stack<string>();
            for (int i = 0; i < lst_infix.Count; i++)
            {
                if (!tool.isOperator(lst_infix[i]))
                    lst_postfix.Add(lst_infix[i]);
                else if (lst_infix[i] == "(") stack.Push(lst_infix[i]);
                else if (lst_infix[i] == ")")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                        lst_postfix.Add(stack.Pop());
                    if (stack.Count > 0 && stack.Peek() == "(")
                        stack.Pop();
                }
                else
                {
                    while (stack.Count > 0 && tool.GetPriority(lst_infix[i]) <= tool.GetPriority(stack.Peek().ToString()))
                        lst_postfix.Add(stack.Pop());
                    stack.Push(lst_infix[i]);
                }
            }
            while (stack.Count > 0)
                lst_postfix.Add(stack.Pop());
            return lst_postfix;
        }
    }

    public class Calculator : Postfix
    {
        private SupportTools tool = new SupportTools();
        private float? result;
        private bool isError = false;
        private List<string> lst_postfix;

        // Hàm Lấy dữ liệu từ người dùng
        public void GetInput(string input)
        {
            base.input = input;
        }

        // Hàm Thực hiện tính toán theo Postfix
        private float? Calculate()
        {
            lst_postfix = base.GetPostfixLst();
            Stack<float> stack = new Stack<float>();
            for (int i = 0; i < lst_postfix.Count; i++)
            {
                float temp = 0;
                if (!tool.isOperator(lst_postfix[i]))
                {
                    float number;
                    bool isNumber = float.TryParse(lst_postfix[i], out number);
                    if (isNumber) stack.Push(number); // Kiểm tra nếu không phải số thoát ctr và báo lỗi
                    else { isError = true; break; }
                }
                else
                {
                    float x, y;
                    x = (float)stack.Pop();
                    if (stack.Count > 0)
                        y = (float)stack.Pop();
                    else
                    {
                        isError = true;
                        break;
                    }
                    if (lst_postfix[i] == "+") temp = y + x;
                    else if (lst_postfix[i] == "-") temp = y - x;
                    else if (lst_postfix[i] == "/") temp = y / x;
                    else if (lst_postfix[i] == "*") temp = y * x;
                    else temp = -1;
                    stack.Push(temp);
                }
            }
            if (stack.Count == 1 && !isError)
                return result = stack.Pop();
            else
            {
                isError = true;
                return result = null;
            }
        }

        // Hàm hiển thị kết quả
        public float? ShowResult()
        {
            result = Calculate();
            return result;
        }
        // Hàm thông báo lỗi
        public string ShowError()
        {
            if (isError) return "Error";
            else return "";
        }

        // Hàm quay tất cả về mặc định
        public void Reset()
        {
            base.lst_infix.Clear();
            base.lst_infix.TrimExcess(); // Xóa Capacity
            this.lst_postfix.Clear(); // lst_postfix đã xóa hết dữ liệu trừ Capity 
            this.lst_postfix.TrimExcess();
            isError = false;
            result = null;
        }
    }

}
