<%@ Page Title="Customer Detail" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Customer_Detail" MaintainScrollPositionOnPostback="true" Debug="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col-6">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">
                        <a runat="server" href="/setting/customer" class="me-2">Customer</a>Detail
                    </h2>
                </div>

                <div class="col-6 text-end">
                    <button type="button" id="btnEdit" class="btn btn-info">
                        Edit
                    </button>
                    <a href="javascript:void(0);" id="aDelete" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                    <a href="javascript:void(0);" id="aCreateOrder" class="btn btn-orange" data-bs-toggle="modal" data-bs-target="#modalCreateOrder">Create Order</a>
                    <a href="javascript:void(0);" id="aLog" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalLog">Log</a>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">

                <div class="card col-12 mb-3">
                    <div class="card-header">
                        <div class="card-title">
                            <span class="text-secondary text-uppercase h4">Customer Name</span> <br>
                            <span class="h3" id="lblCustomerName"></span>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row mb-5">
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Account</span> <br>
                                <span class="h3" id="lblAccount"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Master Customer</span> <br>
                                <span class="h3" id="lblMasterCustomer"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Customer Type</span> <br>
                                <span class="h3" id="lblCustomerType"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Sales Person</span> <br>
                                <span class="h3" id="lblSalesPerson"></span>
                            </div>
                        </div>
                        <div class="row mb-5">
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Web Id</span> <br>
                                <span class="h3" id="lblWebId"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Exact ID</span> <br>
                                <span class="h3" id="lblExactId"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Customer Group</span> <br>
                                <span class="h3" id="lblCustomerGroup"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Customer Price Group</span> <br>
                                <span class="h3" id="lblCustomerPriceGroup"></span>
                            </div>
                        </div>
                        <div class="row mb-5">
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">On Stop</span> <br>
                                <span class="h3" id="lblOnStop"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Cash Sale</span> <br>
                                <span class="h3" id="lblCashSale"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Newsletter</span> <br>
                                <span class="h3" id="lblNewsletter"></span>
                            </div>
                            <div class="col-lg-3">
                                <span class="text-secondary text-uppercase h4">Min Order Surcharge</span> <br>
                                <span class="h3" id="lblMinOrderSurcharge"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Tabs -->
                <div class="card col-12">
                  <div class="card-header">
                    <ul class="nav nav-tabs card-header-tabs nav-fill" data-bs-toggle="tabs">
                      <li class="nav-item">
                        <a href="#tabs-contact" data-tabs="tabs-contact" id="contact" class="nav-link tab-click" data-bs-toggle="tab">CONTACTS</a>
                      </li>
                      <li class="nav-item">
                        <a href="#tabs-address" data-tabs="tabs-address" id="address" class="nav-link tab-click" data-bs-toggle="tab">ADDRESSES</a>
                      </li>
                      <li class="nav-item">
                        <a href="#tabs-logins" data-tabs="tabs-logins" id="logins" class="nav-link tab-click" data-bs-toggle="tab">LOGINS</a>
                      </li>
                      <li class="nav-item">
                        <a href="#tabs-discount" data-tabs="tabs-discount" id="discount" class="nav-link tab-click" data-bs-toggle="tab">DISCOUNT</a>
                      </li>
                      <li class="nav-item">
                        <a href="#tabs-product-access" data-tabs="tabs-product-access" id="product-access" class="nav-link tab-click" data-bs-toggle="tab">PRODUCT ACCESS</a>
                      </li>
                      <li class="nav-item">
                        <a href="#tabs-quotes" data-tabs="tabs-quotes" id="quotes" class="nav-link tab-click" data-bs-toggle="tab">QUOTES</a>
                      </li>
                    </ul>
                  </div>
                  <div class="card-body">
                    <div class="tab-content">

                      <!-- CONTACTS -->
                      <div class="tab-pane" id="tabs-contact">
                        <div class="row">
                          <div class="col-8"></div>
                          <div class="col-4">
                            <button type="button" class="btn btn-danger float-end ms-2" id="btn-reset-primary-contact">Reset Primary Contact</button>
                            <button type="button" class="btn btn-primary float-end" id="btn-create-contact">New Contact</button>
                          </div>
                          <table class="table table-hover table-vcenter card-table w-100 col-12 " id="data-table">
                              <thead class="h1">
                                  <tr>
                                      <th>#</th>
                                      <th>name</th>
                                      <th>Salutation</th>
                                      <th>role</th>
                                      <th>email</th>
                                      <th>phone</th>
                                      <th>mobile</th>
                                      <th>tags</th>
                                      <th>note</th>
                                      <th>primary</th>
                                      <th></th>
                                  </tr>
                              </thead>
                          </table>
                        </div>
                      </div>

                      <!-- ADDRESSES -->
                      <div class="tab-pane" id="tabs-address">
                        <div class="row">
                          <div class="col-8"></div>
                          <div class="col-4">
                            <button type="button" class="btn btn-danger float-end ms-2" id="btn-reset-primary-address">Reset Primary Address</button>
                            <button type="button" class="btn btn-primary float-end" id="btn-create-address">New Address</button>
                          </div>
                          <table class="table table-hover table-vcenter card-table w-100 col-12 " id="data-table">
                              <thead class="h1">
                                  <tr>
                                      <th>#</th>
                                      <th>Description</th>
                                      <th>Address</th>
                                      <th>Nearest Port</th>
                                      <th>Tags</th>
                                      <th>Instruction</th>
                                      <th>Primary</th>
                                      <th></th>
                                  </tr>
                              </thead>
                          </table>
                        </div>
                      </div>
                      
                      <!-- LOGINS -->
                      <div class="tab-pane" id="tabs-logins">
                        <div class="row">
                            <div class="col-8 mb-2"></div>
                            <div class="col-4 mb-2">
                              <button type="button" class="btn btn-azure float-end ms-2" id="btn-reset-primary-login">Email Login Details</button>
                              <button type="button" class="btn btn-primary float-end" id="btn-create-contact">New Login</button>
                            </div>
                            <table class="table table-hover table-vcenter card-table w-100 col-12" id="data-table">
                                <thead class="h1">
                                    <tr>
                                        <th>#</th>
                                        <th>application</th>
                                        <th>role</th>
                                        <th>user</th>
                                        <th>full name</th>
                                        <th>last login</th>
                                        <th>active</th>
                                        <th></th>
                                    </tr>
                                </thead>
                            </table>
                          </div>
                      </div>

                      <!-- DISCOUNT -->
                      <div class="tab-pane" id="tabs-discount">
                        <div class="row">
                          <div class="col-8"></div>
                          <div class="col-4">
                            <button type="button" class="btn btn-secondary float-end ms-2" id="btn-reset-primary-address">Add Custome Discount (Fabric)</button>
                            <button type="button" class="btn btn-primary float-end" id="btn-create-address">Add Discount</button>
                          </div>
                          <table class="table table-hover table-vcenter card-table w-100 col-12 " id="data-table">
                              <thead class="h1">
                                  <tr>
                                      <th>#</th>
                                      <th>id</th>
                                      <th>title</th>
                                      <th>discount</th>
                                      <th>start date</th>
                                      <th>end date</th>
                                      <th>final discount (fabric)</th>
                                      <th></th>
                                  </tr>
                              </thead>
                          </table>
                        </div>
                      </div>

                      <!-- product access -->
                      <div class="tab-pane" id="tabs-product-access">
                        <div class="row">
                          <table class="table table-hover table-vcenter card-table w-100 col-12 " id="data-table">
                              <thead class="h1">
                                  <tr>
                                      <th>Product</th>
                                      <th></th>
                                  </tr>
                              </thead>
                          </table>
                        </div>
                      </div>

                      <!-- quotes -->
                      <div class="tab-pane" id="tabs-quotes">
                        <div class="row">
                          <table class="table table-hover table-vcenter card-table w-100 col-12 " id="data-table">
                              <thead class="h1">
                                  <tr>
                                      <th>Logo</th>
                                      <th>Terms</th>
                                      <th></th>
                                  </tr>
                              </thead>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>


            </div>
        </div>
    </div>    

    <script type="text/javascript">
        let uriMethod = '/Methods/Setting/Customer/CustomerDetailMethod.aspx';
        let roleName = '<%= Session("RoleName") %>';
        let customerDetail = '<%= Session("customerDetail") %>';

        const setState = (name, value) => {
          if (!name && !value) return console.warn("setState: name and value required");
          localStorage.setItem(name, value);
        };

        const getState = (name) => {
          if (!name) return console.warn("getState: name required");
          return localStorage.getItem(name);
        };
    </script>
    <script src="/Scripts/Setting/Customer/Detail.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

