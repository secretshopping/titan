using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prem.PTC;
using Prem.PTC.Members;
using Titan.Matrix;

public abstract class MatrixCrediterBase
{
    public abstract void Credit(Member user, Money servicePrice);
}