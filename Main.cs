﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using final_db_forms.Forms;
using final_db_forms.Properties;

namespace final_db_forms
{
    public partial class Main : Form
    {
        private bool hidden_buttons;
        public Main()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            load_ds();
            init_dgv();
            hidden_buttons=hide_buttons();
            hide_filtering_index();
            init_listbox();
        }
        private void init_dgv()
        {
            dgv_index.AutoGenerateColumns = false;
            dataGridViewTextBoxColumn1.Visible = false;
            dgv_fake_index.Visible = false;
        }
        private void load_ds()
        {
            nep_ingredientsTableAdapter.Fill(llu.nep_ingredients);
            name_of_breadTableAdapter.Fill(llu.name_of_bread);
            priceTableAdapter.Fill(llu.price);
            positionsTableAdapter.Fill(llu.positions);
            food_categoriesTableAdapter.Fill(llu.food_categories);
            ingredientsTableAdapter.Fill(llu.ingredients);
            recipesTableAdapter.Fill(llu.recipes);
            sales_forceTableAdapter.Fill(llu.sales_force);
            soldTableAdapter.Fill(llu.sold);
            sold_recipe_indexTableAdapter.Fill(llu.sold_recipe_index);
        }
        private bool hide_buttons()
        {
            add_emps.Visible = false;
            add_sales.Visible = false;
            return true;
        }
        private void hide_filtering_index()
        {
            if (options_index.Checked == false)
            {
                index_to.Visible = false;
                index_range.Visible = false;
            }
            else
            {
                index_to.Visible = true;
                index_range.Visible = true;
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ta_llu.UpdateAll(llu);
        }
        private void toolStripButton2_Click(object sender, EventArgs e)//add employees
        {
            new Hierarchy(llu,ta_llu).Show();
        }
        private void toolStripButton3_Click(object sender, EventArgs e)//add sales
        {
            var add_sale = new Add_Sale(llu,ta_llu);
            add_sale.Show();
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.TabPages[0] || tabControl1.SelectedTab == tabControl1.TabPages[1] || tabControl1.SelectedTab == tabControl1.TabPages[4])
            {
                if (hidden_buttons!=true)
                {
                    hide_buttons();
                }
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages[2])
            {
                add_sales.Visible = false;
                add_emps.Visible = true;
                hidden_buttons = false;
            }
            else if (tabControl1.SelectedTab == tabControl1.TabPages[3])
            {
                add_emps.Visible = false;
                add_sales.Visible = true;
            }
            
        }
        private void filter_by_date_ValueChanged(object sender, EventArgs e)
        {
            sold_recipe_indexBindingSource.Filter = $"date = '{d_index.Value.Date}'";
        }
        #region(filtering_index)
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            hide_filtering_index();
        }
        private void n_index_up_ValueChanged(object sender, EventArgs e)
        {
        }
        #endregion
        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                apply_filtering_Index();
            }
            catch
            {
                soldBindingSource.RemoveFilter();
                sold_recipe_indexBindingSource.RemoveFilter();
            }
            
        }
        private void apply_filtering_Index()
        {
            var stringbuilder = "";
            try
            {
                switch (checkBox3.Checked)
                {
                    case true:
                    {
                        soldBindingSource.Filter = $"amount = '{n_index_up.Value}'";
                        var indexes =get_data_from_fake_dgv();
                        plow_through(indexes);
                        break;
                    }
                    default:
                    {
                        switch (checkBox1.Checked)
                        {
                            case true when !checkBox2.Checked:
                            {
                                soldBindingSource.Filter = $"amount > '{n_index_up.Value}'";
                                var indexes =get_data_from_fake_dgv();
                                plow_through(indexes);
                                break;
                            }
                            default:
                            {
                                switch (checkBox2.Checked)
                                {
                                    case true when !checkBox1.Checked:
                                    {
                                        soldBindingSource.Filter = $"amount < '{n_index_up.Value}'";
                                        var indexes =get_data_from_fake_dgv();
                                        plow_through(indexes);
                                        break;
                                    }
                                    default:
                                    {
                                        switch (checkBox1.Checked)
                                        {
                                            case true when checkBox2.Checked:
                                            {
                                                soldBindingSource.Filter = $"amount > '{n_index_up.Value}' OR amount < {n_index_down.Value}";
                                                var indexes =get_data_from_fake_dgv();
                                                plow_through(indexes);
                                                break;
                                            }
                                            default:
                                            {
                                                switch (checkBox4.Checked)
                                                {
                                                    case true:
                                                    {
                                                        soldBindingSource.Filter = $"amount = '{n_index_down.Value}' AND amount < {n_index_up.Value}";
                                                        var indexes =get_data_from_fake_dgv();
                                                        plow_through(indexes);
                                                        break;
                                                    }
                                                }

                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                }
                sold_recipe_indexBindingSource.Filter = stringbuilder;
            }
            catch (Exception)
            {
                soldBindingSource.RemoveFilter();
                sold_recipe_indexBindingSource.RemoveFilter();
                MessageBox.Show(Resources.nothing_found);
            }
            
            int[] get_data_from_fake_dgv()
            {
                var indexes = new int[dgv_fake_index.RowCount-1];
                for(int r = 0; r < dgv_fake_index.RowCount-1; r++)
                {
                    indexes[r]=(int) dgv_fake_index[0, r].Value;
                }

                return indexes;
            }
            void plow_through(int[] indexes)
            {
                foreach (var index in indexes)
                {
                    switch (stringbuilder)
                    {
                        case "":
                            stringbuilder =$"id_sold  = {index} " ;
                            break;
                        default:
                            stringbuilder += $"OR id_sold  = {index} ";
                            break;
                    }
                }
            }
        }
        private void dgv_index_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            soldBindingSource.RemoveFilter();
            sold_recipe_indexBindingSource.RemoveFilter();
            MessageBox.Show(Resources.nothing_found);
            
        }
        private void init_listbox()
        {
            var emps = llu.sales_force.Select(row => row.name + " " + row.surname).ToList();
            foreach (var item in emps)
            {
                emp_list.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var stringbuilder = "";
            plow_through(get_emp_id_list());
            soldBindingSource.Filter = stringbuilder;
            void plow_through(int[] indexes)
            {
                foreach (var index in indexes)
                {
                    switch (stringbuilder)
                    {
                        case "":
                            stringbuilder = $"id_sales_force  = {index} ";
                            break;
                        default:
                            stringbuilder += $"OR id_sales_force  = {index} ";
                            break;
                    }
                }
            }
        }
        private int[] get_emp_id_list() 
        {
            int[] id_list = new int[emp_list.CheckedItems.Count];
            if (emp_list.CheckedItems.Count != 0)
            {
                var arr = new string[emp_list.CheckedItems.Count];

                for (int i = 0; i < emp_list.CheckedItems.Count; i++)
                {
                    arr[i] = emp_list.CheckedItems[i].ToString();
                }
                var id_arr = llu.sales_force.Select(id => (id.name + " " + id.surname))
                    .ToArray();
                var common = arr.Intersect(id_arr);
                int v = 0;
                foreach (var item in llu.sales_force)
                {
                    foreach (var sub in common)
                    {
                        if ((item.name + " " + item.surname) == sub)
                        {
                            id_list[v] = item.employee_id;
                            v++;
                        }
                    }
                }
            }
            return id_list;
        }
    }
}