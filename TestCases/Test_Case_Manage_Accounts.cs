﻿using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.Json;
using SpecFlowDemo.TestData;
using SpecFlowDemo.Utilities;
using SpecFlowDemo.PageObjects;

namespace SpecFlowDemo.TestCases
{

    public class Test_Case_Manage_Accounts : PageTest
    {

        private readonly PageObjectAccountEntity pageObjectAccountEntity;
        private readonly Test_Data_Accounts? accountsData;
        private new readonly IPage Page;
        private readonly FormatJsonFile formatJsonFile;

        public Test_Case_Manage_Accounts(IPage Page)
        {
            this.Page = Page;
            formatJsonFile = new FormatJsonFile();
            string MyProjectDir = formatJsonFile.DirProject();
            string filePath = MyProjectDir + "\\TestData\\JsonFiles\\Accounts.json";
            pageObjectAccountEntity = new PageObjectAccountEntity(this.Page);
            string text = File.ReadAllText(filePath);
            accountsData = JsonSerializer.Deserialize<Test_Data_Accounts>(text);
        }

        public async Task NavigateToSalesHubApp()
        {
            await pageObjectAccountEntity.NavigateSalesHub();
        }

        public async Task NavigateToAnEntity(string entityName)
        {
            switch (entityName.ToUpper())
            {
                case "ACCOUNTS":
                    Console.WriteLine("Navigating to Accounts Entity");
                    await pageObjectAccountEntity.NavigateAccounts();
                    break;
                case "CONTACTS":
                    Console.WriteLine("Navigating to ContactsAccounts Entity");
                    break;
                case "CASES":
                    Console.WriteLine("Navigating to Cases Entity");
                    await pageObjectAccountEntity.NavigateCases();
                    break;
                default:
                    Console.WriteLine("No matching data");
                    break;
            }
        }

        public async Task CreateAccount()
        {
            Console.WriteLine(accountsData!.AccountName);
            await pageObjectAccountEntity.AddNewAccount();
            await pageObjectAccountEntity.EnterAccountName(accountsData.AccountName);
            await pageObjectAccountEntity.EnterParentAccountName(accountsData.ParentAccountName);
            await pageObjectAccountEntity.ClickParentSearch();
            await pageObjectAccountEntity.WaitParentPnlVisiblity();
            await pageObjectAccountEntity.GetParentAccountByText(accountsData.ParentAccountName);
            await pageObjectAccountEntity.SaveForm();
            await Expect(pageObjectAccountEntity.BtnSave).ToBeVisibleAsync();
        }

        public async Task SearchAndDeleteAccount()
        {
            Console.WriteLine("Search and delete account - " + accountsData!.AccountName);
            await pageObjectAccountEntity.NavigateAccounts();
            await pageObjectAccountEntity.NavigateAccounts();
            await pageObjectAccountEntity.ClickAccountSearchField();
            await pageObjectAccountEntity.SearchAccount(accountsData.AccountName);
            await pageObjectAccountEntity.ClickAccountSearch();
            for (int i = 0; i < 10; i++)
            {
                await pageObjectAccountEntity.BtnRefresh.WaitForAsync();
                await pageObjectAccountEntity.BtnRefresh.ClickAsync();
                Thread.Sleep(1000);
            }
            await pageObjectAccountEntity.SelectAccountSearch(accountsData.AccountName);
            string accountNameField = await pageObjectAccountEntity.GetAccountName().InnerTextAsync();
            Console.WriteLine("The name of the account - " + accountNameField);
            await pageObjectAccountEntity.ClickMoreCommands();
            await pageObjectAccountEntity.ClickDelete();
            await pageObjectAccountEntity.ClickConfirm();
        }
    }
}
