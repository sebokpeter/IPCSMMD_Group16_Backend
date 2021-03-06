﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ipcsmmd_webshop.Core.ApplicationService;
using ipcsmmd_webshop.Core.ApplicationService.Impl;
using ipcsmmd_webshop.Core.DomainService;
using ipcsmmd_webshop.Core.Entity;
using Moq;
using Xunit;
using System.Linq;

namespace TestCore.ApplicationService.Impl
{
    public class BeerServiceTest
    {

        #region CreateTests
        [Fact]
        public void CreateNewBeerWithNullBeerThrowsException()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService beerService = new BeerService(repo.Object);
            Beer newBeer = null;

            Exception ex = Assert.Throws<InvalidDataException>(() => beerService.AddBeer(newBeer));
            Assert.Equal("Input is null!", ex.Message);
        }

        [Fact]
        public void CreateNewBeerWithIDThrowsException()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService beerService = new BeerService(repo.Object);
            Beer newBeer = new Beer()
            {
                ID = 1,
                Name = "Best_Beer",
                Brand = "Best_Brand",
                Percentage = 50f,
                Type = BeerType.Dark
            };

            Exception ex = Assert.Throws<InvalidDataException>(() => beerService.AddBeer(newBeer));
            Assert.Equal("Cannot add a Beer with existing id!", ex.Message);
        }

        [Fact]
        public void CreateNewBeerWithoutNameThrowsException()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService beerService = new BeerService(repo.Object);
            Beer newBeer = new Beer()
            {
                Brand = "Best_Brand",
                Percentage = 50f,
                Price = 1f,
                Type = BeerType.Dark
            };

            Exception ex = Assert.Throws<InvalidDataException>(() => beerService.AddBeer(newBeer));
            Assert.Equal("Cannot add a Beer without name!", ex.Message);
        }

        [Fact]
        public void CreateNewBeerWithoutPriceThrowsException()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService beerService = new BeerService(repo.Object);
            Beer newBeer = new Beer()
            {
                Name = "Best_Beer",
                Brand = "Best_Brand",
                Percentage = 50f,
                Type = BeerType.Dark
            };

            Exception ex = Assert.Throws<InvalidDataException>(() => beerService.AddBeer(newBeer));
            Assert.Equal("Cannot add a Beer without price!", ex.Message);
        }

        [Fact]
        public void CreateNewBeerWithoutBrandThrowsException()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService beerService = new BeerService(repo.Object);
            Beer newBeer = new Beer()
            {
                Name = "Best_Beer",
                Percentage = 50f,
                Type = BeerType.Dark
            };

            Exception ex = Assert.Throws<InvalidDataException>(() => beerService.AddBeer(newBeer));
            Assert.Equal("Cannot add a Beer without brand!", ex.Message);
        }

        [Fact]
        public void CreateNewBeerShouldCallBeerRepoSaveMethodOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService beerService = new BeerService(repo.Object);

            Beer newBeer = new Beer()
            {
                Name = "Best_Beer",
                Brand = "Best_Brand",
                Percentage = 50f,
                Price = 1d,
                Type = BeerType.Dark
            };

            beerService.AddBeer(newBeer);
            repo.Verify(x => x.Save(It.IsAny<Beer>()), Times.Once);
        }
        #endregion

        #region GetByIDTests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void GetByIDInvalidIDThrowsException(int id)
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);
            Exception ex = Assert.Throws<InvalidDataException>(() => service.GetBeerByID(id));
            Assert.Equal("ID must be greater than 0!", ex.Message);
        }

        [Fact]
        public void GetByIDShouldCallRepoGetByIDOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            service.GetBeerByID(1);
            repo.Verify(x => x.GetByID(1), Times.Once);
        }

        #endregion

        #region GetAllTest

        [Fact]
        public void GetBeersShouldCallRepoGetAllOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            service.GetBeers();
            repo.Verify(x => x.GetAll(), Times.Once);
        }

        #endregion

        #region GetBeersByPrice test

        [Fact]
        public void GetBeersByPriceShouldCallRepoGetAllOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            service.GetBeersByPrice(true);
            repo.Verify(x => x.GetAll(), Times.Once);

            repo.Reset();

            service.GetBeersByPrice(false);
            repo.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public void GetBeersByPriceShoulReturnBeersInOrder()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            IEnumerable<Beer> mockBeers = new Beer[] {
                new Beer()
                {
                     Name = "Best_Beer",
                     Brand = "Best_Brand",
                     Percentage = 50f,
                     Price = 1d,
                     Type = BeerType.Dark
                },
                new Beer()
                {
                     Name = "Bestes_Beer",
                     Brand = "Bestes_Brand",
                     Percentage = 50f,
                     Price = 100d,
                     Type = BeerType.Dark
                },
                 new Beer()
                {
                     Name = "Bestes_Beer",
                     Brand = "Bestes_Brand",
                     Percentage = 50f,
                     Price = 50d,
                     Type = BeerType.Dark
                },
                  new Beer()
                {
                     Name = "Bestes_Beer",
                     Brand = "Bestes_Brand",
                     Percentage = 50f,
                     Price = double.MaxValue,
                     Type = BeerType.Dark
                }
            };

            repo.Setup(x => x.GetAll()).Returns(mockBeers);

            List<Beer> beersAscending = service.GetBeersByPrice(true);
            mockBeers = mockBeers.OrderBy(x => x.Price);

            Assert.Equal(mockBeers.ToList(), beersAscending);

            List<Beer> beersDescending = service.GetBeersByPrice(false);
            mockBeers = mockBeers.OrderByDescending(x => x.Price);

            Assert.Equal(mockBeers.ToList(), beersDescending);
        }

        #endregion

        #region GetBeersByTypeTests

        [Fact]
        public void GetBeersByTypeShouldCallRepoGetAllOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            service.GetBeersByType(BeerType.Brown);
            repo.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public void GetBeersByTypeShouldOnlyReturnCorrectTypes()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            IEnumerable<Beer> mockBeers = new Beer[] {
                new Beer()
                {
                     Name = "Best_Beer_Dark_1",
                     Brand = "Best_Brand",
                     Percentage = 50f,
                     Price = 1d,
                    Type = BeerType.Dark
                },
                new Beer()
                {
                     Name = "Best_Beer_Dark_2",
                     Brand = "Best_Brand",
                     Percentage = 50f,
                     Price = 1d,
                    Type = BeerType.Dark
                },
                new Beer()
                {
                     Name = "Bestes_Beer_Brown_1",
                     Brand = "Bestes_Brand",
                     Percentage = 50f,
                     Price = 100d,
                     Type = BeerType.Brown
                },
                 new Beer()
                {
                     Name = "Bestes_Beer_Brown_2",
                     Brand = "Bestes_Brand",
                     Percentage = 50f,
                     Price = 50d,
                     Type = BeerType.Brown
                },
                  new Beer()
                {
                     Name = "Best_Beer_Light_1",
                     Brand = "Best_Brand",
                     Percentage = 50f,
                     Price = 1d,
                    Type = BeerType.Light
                },
                new Beer()
                {
                     Name = "Best_Beer_Light_2",
                     Brand = "Best_Brand",
                     Percentage = 50f,
                     Price = 1d,
                    Type = BeerType.Light
                },
            };

            repo.Setup(x => x.GetAll()).Returns(mockBeers);

            List<Beer> beers = service.GetBeersByType(BeerType.Brown);

            if (!beers.TrueForAll(x => x.Type == BeerType.Brown))
            {
                throw new Xunit.Sdk.XunitException($"Incorrect type.\nExpected: {BeerType.Brown}");
            }

            beers = service.GetBeersByType(BeerType.Dark);

            if (!beers.TrueForAll(x => x.Type == BeerType.Dark))
            {
                throw new Xunit.Sdk.XunitException($"Incorrect type.\nExpected: {BeerType.Dark}");
            }

            beers = service.GetBeersByType(BeerType.Light);

            if (!beers.TrueForAll(x => x.Type == BeerType.Light))
            {
                throw new Xunit.Sdk.XunitException($"Incorrect type.\nExpected: {BeerType.Light}");
            }

        }

        #endregion

        #region GetFilteredBeersTests

        [Fact]
        public void GetFilteredBeersShouldCallRepoGetFilteredOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            BeerFilter filter = new BeerFilter()
            {
                CurrentPage = 1,
                ItemsPerPage = 10,
                IsAscending = true,
                SearchField = BeerFilter.Field.Id
            };

            service.GetFilteredBeers(filter);
            repo.Verify(x => x.GetFiltered(filter), Times.Once);

        }

        #endregion

        #region RemoveBeerTest

        [Fact]
        public void RemoveBeerShouldCallRepoRemoveOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            Beer beerToRemove = new Beer()
            {
                Name = "Best_Beer_Dark_1",
                Brand = "Best_Brand",
                Percentage = 50f,
                Price = 1d,
                Type = BeerType.Dark
            };

            service.RemoveBeer((int)beerToRemove.ID);
            repo.Verify(x => x.Remove((int)beerToRemove.ID));
        }

        #endregion

        #region UpdateBeerTest

        [Fact]
        public void UpdateBeerShouldCallRepoUpdateOnce()
        {
            var repo = new Mock<IBeerRepository>();
            IBeerService service = new BeerService(repo.Object);

            Beer beerToUpdate = new Beer()
            {
                Name = "Best_Beer_Dark_1",
                Brand = "Best_Brand",
                Percentage = 50f,
                Price = 1d,
                Type = BeerType.Dark
            };

            service.UpdateBeer(beerToUpdate);
            repo.Verify(x => x.Update( beerToUpdate));
        }

        #endregion
    }
}