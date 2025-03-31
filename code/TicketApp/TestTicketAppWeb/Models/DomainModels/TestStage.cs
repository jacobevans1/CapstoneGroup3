using System.ComponentModel.DataAnnotations;
using TicketAppWeb.Models.DomainModels;
using TicketAppWeb.Models.DomainModels.MiddleTableModels;

namespace TestTicketAppWeb.Models.DomainModels
{
	public class StageTests
	{
		[Fact]
		public void Stage_Property_ShouldSetAndGetValues()
		{
			// Arrange
			var stage = new Stage
			{
				Id = "1",
				Name = "Development"
			};

			// Act & Assert
			Assert.Equal("1", stage.Id);
			Assert.Equal("Development", stage.Name);
		}

		[Fact]
		public void Stage_Name_ShouldBeRequired()
		{
			// Arrange
			var stage = new Stage
			{
				Id = "1",
				Name = null
			};

			// Act
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(stage);
			var isValid = Validator.TryValidateObject(stage, validationContext, validationResults, true);

			// Assert
			Assert.False(isValid);
			Assert.Contains(validationResults, v => v.ErrorMessage == "Please provide a stage name");
		}

		[Fact]
		public void Stage_Name_ShouldNotBeEmpty()
		{
			// Arrange
			var stage = new Stage
			{
				Id = "1",
				Name = ""
			};

			// Act
			var validationResults = new List<ValidationResult>();
			var validationContext = new ValidationContext(stage);
			var isValid = Validator.TryValidateObject(stage, validationContext, validationResults, true);

			// Assert
			Assert.False(isValid);
			Assert.Contains(validationResults, v => v.ErrorMessage == "Please provide a stage name");
		}

		[Fact]
		public void Stage_Constructor_ShouldInitializeBoardStagesCollection()
		{
			// Arrange & Act
			var stage = new Stage();

			// Assert
			Assert.NotNull(stage.BoardStages);
			Assert.IsType<List<BoardStage>>(stage.BoardStages);
			Assert.Empty(stage.BoardStages);
		}

		[Fact]
		public void Stage_Collection_AddingBoardStage_ShouldWork()
		{
			// Arrange
			var stage = new Stage();
			var boardStage = new BoardStage { StageId = "S1", StageOrder = 1 };

			// Act
			stage.BoardStages.Add(boardStage);

			// Assert
			Assert.Contains(boardStage, stage.BoardStages);
			Assert.Equal(1, stage.BoardStages.Count);
		}

		[Fact]
		public void Stage_Collection_RemovingBoardStage_ShouldWork()
		{
			// Arrange
			var stage = new Stage();
			var boardStage = new BoardStage { StageId = "S1", StageOrder = 1 };
			stage.BoardStages.Add(boardStage);

			// Act
			stage.BoardStages.Remove(boardStage);

			// Assert
			Assert.DoesNotContain(boardStage, stage.BoardStages);
			Assert.Empty(stage.BoardStages);
		}

		[Fact]
		public void Stage_Collection_ShouldNotBeNullOrEmpty_Initially()
		{
			// Arrange
			var stage = new Stage();

			// Act
			var collectionIsEmpty = stage.BoardStages.Count == 0;

			// Assert
			Assert.True(collectionIsEmpty);
		}
	}
}
