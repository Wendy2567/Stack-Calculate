using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnGiaiThuat2
{
    public partial class Form1 : Form
    {
     
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.KeyPreview = true;
        }
 
        //--------------------------------- PHẦN Giao Diện ----------------------------//
        private Dictionary<string, Color> colors = new Dictionary<string, Color>(); // Lưu màu ban đầu của buttons
        private bool isFirst = false; // Biến kiểm tra người dùng lần đầu nhập dữ liệu????
        private Button preBtn = new Button(); // Biến lưu nút người dùng bấm (dùng kiểm tra ng dùng nhập 2 lần giống nhau)
        public Calculator calculator = new Calculator();
        private float? result = null;
 
        private void Form1_Load(object sender, EventArgs e)
        {
            ActiveControl = displayBox;
            foreach (var btn in this.Controls.OfType<Button>())
            {
                colors.Add(btn.Text, btn.BackColor);

                btn.MouseEnter += new EventHandler(btn_MouseEnter);
                btn.MouseLeave += new EventHandler(btn_MouseLeave);
            }
        }

        // Tạo hiệu ứng cho chuột khi rơ chuột
        private void btn_MouseEnter(object sender, System.EventArgs e)
        {
            Color color = (sender as Button).BackColor;
            (sender as Button).BackColor = Color.FromArgb(140, ControlPaint.Dark(color));
        }
        private void btn_MouseLeave(object sender, System.EventArgs e)
        {
            foreach (KeyValuePair<string, Color> color in colors)
            {
                if (color.Key == (sender as Button).Text)
                    (sender as Button).BackColor = color.Value;
            }

        }

        // Khởi tạo sự kiện CLICK ENTER 
        private void btnEnter_Click(object sender, EventArgs e)
        {
            calculator.GetInput(displayBox.Text);
            result = calculator.ShowResult();
            string error = calculator.ShowError();

            if (result != null && error == "") // Trường hợp không lỗi --> lưu kết quả và trở về trạng thái mặc định
            {
                calculator.Reset();
                displayBox.Text = result.ToString();
                isFirst = false;
            }
            else if (error == "Error") // Trường hợp có lỗi --> Về trạng thái mặc định + Hiển thị thông báo lỗi
            {
                calculator.Reset();
                displayBox.Text = error;
            }
            else
                displayBox.Text = result.ToString();
            btnDelete.Enabled = false;
        }


        // PT xóa tất cả các dữ liệu người dùng nhập --> Về mặc định và xóa kết quả lưu trữ
        private void DeleteAll()
        {
            displayBox.Text = "";
            isFirst = false;
            preBtn.Enabled = true;
        }

        // Khởi tạo sự kiện CLICK ENTER 
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            this.DeleteAll();
        }

        // PT xóa từng kí tự 
        private void DeleteOneChar()
        {

            if (displayBox.Text.Length > 0)
            {
                displayBox.Text = displayBox.Text.Remove(displayBox.Text.Length - 1, 1);
                preBtn.Enabled = true;
            }
            else
                isFirst = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.DeleteOneChar();
        }

        // PT kiểm tra kí tự ngoặc đơn
        private bool isParentheses(string value)
        {
            if (value == "(" || value == ")") return true;
            else return false;
        }
        private bool isOperator(string input)
        {
            if (input != "+" && input != "-" && input != "*" && input != "/" && input != "^" && input != "(" && input != ")")
                return false;
            else return true;
        }

        // PT giới hạn thao tác người dùng
        private void HandleUserAction(object sender, string value, float? ans, ref bool isReplace, ref bool isRepeat)
        {
            if ((displayBox.Text.Length > 0) // Trường hợp nhấn liên tiếp phép toán 2 lần
                && isOperator(displayBox.Text[displayBox.Text.Length - 1].ToString()) && isOperator(value)
                && !isParentheses(value) && !isParentheses(displayBox.Text[displayBox.Text.Length - 1].ToString()))
            {
                if ((displayBox.Text[displayBox.Text.Length - 1].ToString()).Equals(value) && value != "-")
                // Trường hợp nhấn liên tiếp 2 lần phép toán giống nhau (Trừ "-")
                {
                    isRepeat = true;
                    preBtn = (Button)sender;
                }
                // Trường hợp nhấn liên tiếp 2 lần phép toán khác nhau
                else if (!(displayBox.Text[displayBox.Text.Length - 1].ToString()).Equals(value))
                { 
                    int posChange = displayBox.Text.Length - 1;
                    displayBox.Text = displayBox.Text.Remove(posChange, 1) + value;
                    isReplace = true;
                }
             }
            else
            {
                // Tiếp tục tính toán với kết quả tính được ở lần trước
                if (!isFirst && ans != null)
                {
                    // Trường hợp nhập số
                    if (Char.IsDigit(value.ToCharArray()[0]))
                        displayBox.Text = "";
                    calculator.Reset();
                }
                isFirst = true;
            }
        }
     

        // PT thực hiện thao tác nhấn trên Calculator
        private void btn_Click(object sender, EventArgs e, float? ans)
        {
            btnDelete.Enabled = true;

            string clickValue = (sender as Button).Text.ToString();
            bool isReplace = false; // Biến ràng buộc không để người dùng nhập 2 phép toán "KHÁC NHAU "cùng lúc bằng pp thay thế (Trừ "-")
            bool isRepeat = false; // Biến ràng buộc không để người dùng nhập 2 phép toán "GIỐNG NHAU"cùng lúc bằng pp chặn nút bấm (Trừ "-")

            HandleUserAction(sender, clickValue, ans, ref isReplace, ref isRepeat);
            if (isReplace || isRepeat) displayBox.Text = displayBox.Text.Remove(displayBox.Text.Length - 1) + clickValue;
            else displayBox.Text += clickValue;
            
            if (isRepeat) preBtn.Enabled = false;
            else preBtn.Enabled = true;
        }
    }
}
