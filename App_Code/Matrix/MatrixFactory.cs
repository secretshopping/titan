using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;

public static class MatrixFactory
{
    private static MatrixBase matrix = null;

    public static MatrixBase GetMatrix()
    {
        try
        {
            switch (AppSettings.Matrix.Type)
            {
                case MatrixType.None:
                    matrix = null;
                    break;
                case MatrixType.Campaing:
                    matrix = new CampaingMatrix();
                    break;
                case MatrixType.Referral:
                    matrix = new ReferralMatrix();

                    break;
                default:
                    throw new MsgException("Unuknown Matrix Type: " + (AppSettings.Matrix.Type.ToString() ?? "NULL"));
            }

            if (matrix != null)
                matrix.Crediter = GetMatrixCrediter(matrix);

            return matrix;
        }
        catch(Exception ex)
        {
            ErrorLogger.Log(ex);
            return matrix;
        }
    }

    private static MatrixCrediterBase GetMatrixCrediter(MatrixBase matrix)
    {
        MatrixCrediterBase crediter = null;

        switch(AppSettings.Matrix.Crediter)
        {
            case MatrixCrediter.None:
                crediter = null;
                break;
            case MatrixCrediter.Commission:
                crediter = new CommissionMatrixCrediter();
                break;
            case MatrixCrediter.CommissionReferrals:
                crediter = new CommissionReferralMatrixCrediter();
                break;
            case MatrixCrediter.Cycles:
                crediter = new BinaryReferralMatrixCrediter();
                break;
            default:
                throw new MsgException("Unuknown Matrix Crediter Type: " + (AppSettings.Matrix.Crediter.ToString() ?? "NULL"));
        }

        return crediter;
    }
}