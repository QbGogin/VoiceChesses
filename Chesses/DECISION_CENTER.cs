using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public class Predict

{

    public static string __Spreadshe_MLP_924_111_8(double[] ContInputs)

    {
        //"Input Variable" comment is added beside Input(Response) variables.
        StreamReader sr = new StreamReader("all/MLP.txt");
        double[] __statist_inputs = new double[924];
        for (int i = 0; i < ContInputs.Length; i++)
        {
            __statist_inputs[i] = ContInputs[i];
        }
        string __statist_PredCat = "";
        string[] __statist_DCats = new string[8] { "1", "2", "3", "4", "5", "6", "7", "8" };
        double __statist_ConfLevel = 3.0E-300;
        double[] __statist_max_input = new double[924];
        for (int i = 0; i < __statist_max_input.Length; i++)
        {
            __statist_max_input[i] = Double.Parse(sr.ReadLine());
        }
        double[] __statist_min_input = new double[924];

        for (int i = 0; i < __statist_min_input.Length; i++)
        {
            __statist_min_input[i] = Double.Parse(sr.ReadLine());
        }

        double[,] __statist_i_h_wts = new double[111, 924];
        for (int i = 0; i < 111; i++)
        {
            for (int j = 0; j < 924; j++)
            {
                __statist_i_h_wts[i, j] = Double.Parse(sr.ReadLine());
            }
        }

        double[,] __statist_h_o_wts = new double[8, 111];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 111; j++)
            {
                __statist_h_o_wts[i, j] = Double.Parse(sr.ReadLine());
            }
        }

        double[] __statist_hidden_bias = new double[111];

        for (int i = 0; i < __statist_hidden_bias.Length; i++)
        {
            __statist_hidden_bias[i] = Double.Parse(sr.ReadLine());
        }

        double[] __statist_output_bias = new double[8];
        for (int i = 0; i < __statist_output_bias.Length; i++)
        {
            __statist_output_bias[i] = Double.Parse(sr.ReadLine());
        }

        double[] __statist_hidden = new double[111];
        double[] __statist_outputs = new double[8];
        __statist_outputs[0] = -1.0e+307;
        __statist_outputs[1] = -1.0e+307;
        __statist_outputs[2] = -1.0e+307;
        __statist_outputs[3] = -1.0e+307;
        __statist_outputs[4] = -1.0e+307;
        __statist_outputs[5] = -1.0e+307;
        __statist_outputs[6] = -1.0e+307;
        __statist_outputs[7] = -1.0e+307;
        double __statist_delta = 0;
        double __statist_maximum = 1;
        double __statist_minimum = 0;
        int __statist_ncont_inputs = 924;
        /*scale continuous inputs*/
        for (int __statist_i = 0; __statist_i < __statist_ncont_inputs; __statist_i++)
        {
            __statist_delta = (__statist_maximum - __statist_minimum) / (__statist_max_input[__statist_i] - __statist_min_input[__statist_i]);
            __statist_inputs[__statist_i] = __statist_minimum - __statist_delta * __statist_min_input[__statist_i] + __statist_delta * __statist_inputs[__statist_i];
        }
        int __statist_ninputs = 924;
        int __statist_nhidden = 111;
        /*Compute feed forward signals from Input layer to hidden layer*/

        for (int __statist_row = 0; __statist_row < __statist_nhidden; __statist_row++)
        {
            __statist_hidden[__statist_row] = 0.0;
            for (int __statist_col = 0; __statist_col < __statist_ninputs; __statist_col++)
            {
                __statist_hidden[__statist_row] = __statist_hidden[__statist_row] + (__statist_i_h_wts[__statist_row, __statist_col] * __statist_inputs[__statist_col]);
            }
            __statist_hidden[__statist_row] = __statist_hidden[__statist_row] + __statist_hidden_bias[__statist_row];
        }
        for (int __statist_row = 0; __statist_row < __statist_nhidden; __statist_row++)
        {
            if (__statist_hidden[__statist_row] > 100.0)
            {
                __statist_hidden[__statist_row] = 1.0;
            }
            else
            {
                if (__statist_hidden[__statist_row] < -100.0)
                {
                    __statist_hidden[__statist_row] = 0.0;
                }
                else
                {
                    __statist_hidden[__statist_row] = 1.0 / (1.0 + Math.Exp(-__statist_hidden[__statist_row]));
                }
            }
        }
        int __statist_noutputs = 8;
        /*Compute feed forward signals from hidden layer to output layer*/
        for (int __statist_row2 = 0; __statist_row2 < __statist_noutputs; __statist_row2++)
        {
            __statist_outputs[__statist_row2] = 0.0;
            for (int __statist_col2 = 0; __statist_col2 < __statist_nhidden; __statist_col2++)
            {
                __statist_outputs[__statist_row2] = __statist_outputs[__statist_row2] + (__statist_h_o_wts[__statist_row2, __statist_col2] * __statist_hidden[__statist_col2]);
            }
            __statist_outputs[__statist_row2] = __statist_outputs[__statist_row2] + __statist_output_bias[__statist_row2];
        }
        double __statist_sum = 0.0;
        double __statist_maxIndex = 0;
        for (int __statist_jj = 0; __statist_jj < __statist_noutputs; __statist_jj++)
        {
            if (__statist_outputs[__statist_jj] > 200)
            {
                double __statist_max = __statist_outputs[1];
                __statist_maxIndex = 0;
                for (int __statist_ii = 0; __statist_ii < __statist_noutputs; __statist_ii++)
                {
                    if (__statist_outputs[__statist_ii] > __statist_max)
                    {
                        __statist_max = __statist_outputs[__statist_ii];
                        __statist_maxIndex = __statist_ii;
                    }
                }
                for (int __statist_kk = 0; __statist_kk < __statist_noutputs; __statist_kk++)
                {
                    if (__statist_kk == __statist_maxIndex)
                    {
                        __statist_outputs[__statist_jj] = 1.0;
                    }
                    else
                    {
                        __statist_outputs[__statist_kk] = 0.0;
                    }
                }
            }
            else
            {
                __statist_outputs[__statist_jj] = Math.Exp(__statist_outputs[__statist_jj]);
                __statist_sum = __statist_sum + __statist_outputs[__statist_jj];
            }
        }
        for (int __statist_ll = 0; __statist_ll < __statist_noutputs; __statist_ll++)
        {
            if (__statist_sum != 0)
            {
                __statist_outputs[__statist_ll] = __statist_outputs[__statist_ll] / __statist_sum;
            }
        }
        int __statist_PredIndex = 1;
        for (int __statist_ii = 0; __statist_ii < __statist_noutputs; __statist_ii++)
        {
            if (__statist_ConfLevel < __statist_outputs[__statist_ii])
            {
                __statist_ConfLevel = __statist_outputs[__statist_ii];
                __statist_PredIndex = __statist_ii;
            }
        }
        __statist_PredCat = __statist_DCats[__statist_PredIndex];
        sr.Close();
        return (__statist_PredCat);
    }

    public static string __Spreadshe_MLP_924_112_8(double[] arr_inputs)

    {
        StreamReader sr = new StreamReader("all/MLP11.txt");
        double[] inputs = new double[924];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = arr_inputs[i];
        }
        string predict = "";
        string[] predict_category = new string[8] { "1", "2", "3", "4", "5", "6", "7", "8" };
        double conf_level = 3.0E-300;
        double[] max_inputs = new double[924];
        for (int i = 0; i < max_inputs.Length; i++)
        {
            max_inputs[i] = Double.Parse(sr.ReadLine());
        }
        double[] min_inputs = new double[924];
        for (int i = 0; i < min_inputs.Length; i++)
        {
            min_inputs[i] = Double.Parse(sr.ReadLine());
        }
        double[,] input_hidden_weights = new double[112, 924];
        for (int i = 0; i < 112; i++)
        {
            for (int j = 0; j < 924; j++)
            {
                input_hidden_weights[i, j] = Double.Parse(sr.ReadLine());
            }
        }
        double[,] hidden_output_weights = new double[8, 112];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 112; j++)
            {
                hidden_output_weights[i, j] = Double.Parse(sr.ReadLine());
            }
        }
        double[] hidden_bias = new double[112];
        for (int i = 0; i < hidden_bias.Length; i++)
        {
            hidden_bias[i] = Double.Parse(sr.ReadLine());
        }
        double[] output_bias = new double[8];
        for (int i = 0; i < output_bias.Length; i++)
        {
            output_bias[i] = Double.Parse(sr.ReadLine());
        }
        double[] hidden = new double[112];
        double[] outputs = new double[8];
        outputs[0] = -1.0e+307;
        outputs[1] = -1.0e+307;
        outputs[2] = -1.0e+307;
        outputs[3] = -1.0e+307;
        outputs[4] = -1.0e+307;
        outputs[5] = -1.0e+307;
        outputs[6] = -1.0e+307;
        outputs[7] = -1.0e+307;
        double Delta = 0;
        double maximum = 1;
        double minimum = 0;
        int number_of_inputs = 924;
        /*scale continuous inputs*/
        for (int i = 0; i < number_of_inputs; i++)
        {
            Delta = (maximum - minimum) / (max_inputs[i] - min_inputs[i]);
            inputs[i] = minimum - Delta * min_inputs[i] + Delta * inputs[i];
        }
        int numb_inputs = 924;
        int numb_hidden = 112;
        /*Compute feed forward signals from Input layer to hidden layer*/
        for (int row = 0; row < numb_hidden; row++)
        {
            hidden[row] = 0.0;
            for (int col = 0; col < numb_inputs; col++)
            {
                hidden[row] = hidden[row] + (input_hidden_weights[row, col] * inputs[col]);
            }
            hidden[row] = hidden[row] + hidden_bias[row];
        }
        for (int row = 0; row < numb_hidden; row++)
        {
            if (hidden[row] > 100.0)
            {
                hidden[row] = 1.0;
            }
            else
            {
                if (hidden[row] < -100.0)
                {
                    hidden[row] = -1.0;
                }
                else
                {
                    hidden[row] = Math.Tanh(hidden[row]);
                }
            }
        }
        int numb_outputs = 8;
        /*Compute feed forward signals from hidden layer to output layer*/
        for (int row2 = 0; row2 < numb_outputs; row2++)
        {
            outputs[row2] = 0.0;
            for (int col2 = 0; col2 < numb_hidden; col2++)
            {
                outputs[row2] = outputs[row2] + (hidden_output_weights[row2, col2] * hidden[col2]);
            }
            outputs[row2] = outputs[row2] + output_bias[row2];
        }
        double sum = 0.0;
        double max_ind = 0;
        for (int jj = 0; jj < numb_outputs; jj++)
        {
            if (outputs[jj] > 200)
            {
                double now_max = outputs[1];
                max_ind = 0;
                for (int ii = 0; ii < numb_outputs; ii++)
                {
                    if (outputs[ii] > now_max)
                    {
                        now_max = outputs[ii];
                        max_ind = ii;
                    }
                }
                for (int kk = 0; kk < numb_outputs; kk++)
                {
                    if (kk == max_ind)
                    {
                        outputs[jj] = 1.0;
                    }
                    else
                    {
                        outputs[kk] = 0.0;
                    }
                }
            }
            else
            {
                outputs[jj] = Math.Exp(outputs[jj]);
                sum = sum + outputs[jj];
            }
        }
        for (int ll = 0; ll < numb_outputs; ll++)
        {
            if (sum != 0)
            {
                outputs[ll] = outputs[ll] / sum;
            }
        }
        int pred_ind = 1;
        for (int ii = 0; ii < numb_outputs; ii++)
        {
            if (conf_level < outputs[ii])
            {
                conf_level = outputs[ii];
                pred_ind = ii;
            }
        }
        predict = predict_category[pred_ind];
        sr.Close();
        return (predict);

    }
}