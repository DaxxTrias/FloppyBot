using FakeItEasy;
using FloppyBot.Base.TextFormatting;
using FloppyBot.Commands.Core.Entities;
using FloppyBot.Commands.Custom.Execution.Administration;
using FloppyBot.Commands.Custom.Storage;
using FloppyBot.Commands.Custom.Storage.Entities;

namespace FloppyBot.Commands.Custom.Tests.Execution;

[TestClass]
public class CustomCommandAdministrationCommandsTests
{
    private readonly ICounterStorageService _counterStorageService;
    private readonly ICustomCommandService _customCommandService;
    private readonly CustomCommandAdministrationCommands _host;

    public CustomCommandAdministrationCommandsTests()
    {
        _counterStorageService = A.Fake<ICounterStorageService>();
        _customCommandService = A.Fake<ICustomCommandService>();
        _host = new CustomCommandAdministrationCommands(
            _customCommandService,
            _counterStorageService
        );
    }

    [TestMethod]
    public void CreateCommand()
    {
        A.CallTo(
                () =>
                    _customCommandService.CreateSimpleCommand(
                        A<string>.Ignored,
                        A<string>.Ignored,
                        A<string>.Ignored
                    )
            )
            .ReturnsLazily(() => true);

        CommandResult result = _host.CreateCommand(
            "Mock/UnitTest",
            "mycommand",
            "This is my cool command"
        );

        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Success,
                CustomCommandAdministrationCommands.REPLY_CREATE_SUCCESS.Format(
                    new { CommandName = "mycommand" }
                )
            ),
            result
        );
        A.CallTo(
                () =>
                    _customCommandService.CreateSimpleCommand(
                        "Mock/UnitTest",
                        "mycommand",
                        "This is my cool command"
                    )
            )
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public void CreateExisting()
    {
        A.CallTo(
                () =>
                    _customCommandService.CreateSimpleCommand(
                        A<string>.Ignored,
                        A<string>.Ignored,
                        A<string>.Ignored
                    )
            )
            .ReturnsLazily(() => false);

        CommandResult result = _host.CreateCommand(
            "Mock/UnitTest",
            "mycommand",
            "This is my cool command"
        );

        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Failed,
                CustomCommandAdministrationCommands.REPLY_CREATE_FAILED.Format(
                    new { CommandName = "mycommand" }
                )
            ),
            result
        );
        A.CallTo(
                () =>
                    _customCommandService.CreateSimpleCommand(
                        "Mock/UnitTest",
                        "mycommand",
                        "This is my cool command"
                    )
            )
            .MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public void DeleteCommand()
    {
        A.CallTo(() => _customCommandService.DeleteCommand(A<string>.Ignored, A<string>.Ignored))
            .ReturnsLazily(() => true);

        CommandResult result = _host.DeleteCommand("Mock/UnitTest", "mycommand");
        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Success,
                CustomCommandAdministrationCommands.REPLY_DELETE_SUCCESS.Format(
                    new { CommandName = "mycommand" }
                )
            ),
            result
        );
    }

    [TestMethod]
    public void DeleteFailed()
    {
        A.CallTo(() => _customCommandService.DeleteCommand(A<string>.Ignored, A<string>.Ignored))
            .ReturnsLazily(() => false);

        CommandResult result = _host.DeleteCommand("Mock/UnitTest", "mycommand");
        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Failed,
                CustomCommandAdministrationCommands.REPLY_DELETE_FAILED.Format(
                    new { CommandName = "mycommand" }
                )
            ),
            result
        );
    }

    [TestMethod]
    public void SetCounterAbsolute()
    {
        A.CallTo(() => _customCommandService.GetCommand("Mock/UnitTest", "mycommand"))
            .ReturnsLazily(() => new CustomCommandDescription { Id = "abc123" });
        A.CallTo(() => _counterStorageService.Peek("abc123")).ReturnsLazily(() => 5);

        CommandResult result = _host.SetCounter("Mock/UnitTest", "mycommand", "5");
        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Success,
                CustomCommandAdministrationCommands.REPLY_COUNTER_SET.Format(
                    new { CommandName = "mycommand", Counter = 5 }
                )
            ),
            result
        );
        A.CallTo(() => _counterStorageService.Set("abc123", 5)).MustHaveHappenedOnceExactly();
    }

    [DataTestMethod]
    [DataRow("+5", 5)]
    [DataRow("-10", -10)]
    public void SetCounterRelative(string input, int expectedIncrement)
    {
        A.CallTo(() => _customCommandService.GetCommand("Mock/UnitTest", "mycommand"))
            .ReturnsLazily(() => new CustomCommandDescription { Id = "abc123" });
        A.CallTo(() => _counterStorageService.Increase(A<string>.Ignored, An<int>.Ignored))
            .ReturnsLazily((string _, int increment) => 10 + increment);

        CommandResult result = _host.SetCounter("Mock/UnitTest", "mycommand", input);
        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Success,
                CustomCommandAdministrationCommands.REPLY_COUNTER_SET.Format(
                    new { CommandName = "mycommand", Counter = 10 + expectedIncrement }
                )
            ),
            result
        );

        A.CallTo(() => _counterStorageService.Increase("abc123", expectedIncrement))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _counterStorageService.Set("abc123", An<int>.Ignored)).MustNotHaveHappened();
    }

    [TestMethod]
    public void SetCounterClear()
    {
        A.CallTo(() => _customCommandService.GetCommand("Mock/UnitTest", "mycommand"))
            .ReturnsLazily(() => new CustomCommandDescription { Id = "abc123" });

        CommandResult result = _host.SetCounter("Mock/UnitTest", "mycommand", "clear");
        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Success,
                CustomCommandAdministrationCommands.REPLY_COUNTER_SET.Format(
                    new { CommandName = "mycommand", Counter = 0 }
                )
            ),
            result
        );
        A.CallTo(() => _counterStorageService.Set("abc123", 0)).MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public void SetUnknownCommand()
    {
        A.CallTo(() => _customCommandService.GetCommand("Mock/UnitTest", "mycommand"))
            .ReturnsLazily(() => null);

        CommandResult result = _host.SetCounter("Mock/UnitTest", "mycommand", "clear");
        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Failed,
                CustomCommandAdministrationCommands.REPLY_COMMAND_NOT_FOUND.Format(
                    new { CommandName = "mycommand" }
                )
            ),
            result
        );
        A.CallTo(() => _counterStorageService.Set(A<string>.Ignored, An<int>.Ignored))
            .MustNotHaveHappened();
    }

    [TestMethod]
    public void SetCommandFailed()
    {
        A.CallTo(() => _customCommandService.GetCommand("Mock/UnitTest", "mycommand"))
            .ReturnsLazily(() => new CustomCommandDescription { Id = "abc123" });

        CommandResult result = _host.SetCounter("Mock/UnitTest", "mycommand", "notAThing");
        Assert.AreEqual(
            new CommandResult(
                CommandOutcome.Failed,
                CustomCommandAdministrationCommands.REPLY_COUNTER_PARAMS_UNKNOWN
            ),
            result
        );
        A.CallTo(() => _counterStorageService.Set(A<string>.Ignored, A<int>.Ignored))
            .MustNotHaveHappened();
    }
}
