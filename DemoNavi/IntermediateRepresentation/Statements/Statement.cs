﻿using DemoNavi.IntermediateRepresentation.Semantic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoNavi.IntermediateRepresentation
{
    abstract class Statement
    {
        internal abstract void SemanticValidation(SemanticContext semanticContext);
    }
}
