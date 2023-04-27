﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Contracts.ViewModels;
using BookShop.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace BookShop.ViewModels;
public class AddProductViewModel : ObservableRecipient, INavigationAware
{
    private Product? _item;
    private List<Product> _products;
    private string _imagePreview;
    private bool _isImgLoading = false;
    public Product? Item
    {
        get => _item;
        set => SetProperty(ref _item, value);
    }

    public bool IsLoading
    {
        get => _isImgLoading;
        set => SetProperty(ref _isImgLoading, value);
    }

    public string ImagePreview
    {
        get => _imagePreview ?? "x";
        set
        {
            Item.Image = value;
            SetProperty(ref _imagePreview, value);
        }
    }

    public void OnNavigatedFrom()
    {
    }
    public void OnNavigatedTo(object parameter)
    {
        if (parameter is List<Product> products)
        {
            _products = products.ToList();
        }

        Item = new Product();
    }

    public bool ValidateField(ref string message)
    {
        if (_item.Image == string.Empty)
        {
            message = "Must choose a product image";
            return false;
        }

        if (!_item.ValidateField())
        {
            message = "Must fill all required fields";
            return false;
        }

        if (_products.Find(product => product.IsSameProduct(_item)) != null)
        {
            message = "Product is already existed";
            return false;
        }
        return true;
    }
}
