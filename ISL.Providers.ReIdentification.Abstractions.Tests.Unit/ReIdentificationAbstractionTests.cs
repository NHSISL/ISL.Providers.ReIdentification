// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Moq;

namespace ISL.Providers.ReIdentification.Abstractions.Tests.Unit
{
    public partial class ReIdentificationAbstractionTests
    {
        private readonly Mock<IReIdentificationProvider> reIdentificationMock;
        private readonly ReIdentificationAbstractionProvider notificationAbstractionProvider;

        public ReIdentificationAbstractionTests()
        {
            this.reIdentificationMock = new Mock<IReIdentificationProvider>();

            this.notificationAbstractionProvider =
                new ReIdentificationAbstractionProvider(this.reIdentificationMock.Object);
        }
    }
}
