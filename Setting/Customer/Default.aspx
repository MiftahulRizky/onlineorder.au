<%@ Page Title="Customer" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Customer_Default" MaintainScrollPositionOnPostback="true" Debug="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Customer</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="card col-12" id="card-table">
                    <div class="card-header row">
                        <div class="col-lg-10">
                            <h3 class="card-title">Data Customer</h3>
                        </div>
                        <div class="col-lg-2">
                            <button class="btn btn-primary w-100" id="btn-create-new">
                                <i class="ti ti-plus opacity-50 fs-3 me-1"></i>
                                New Customer
                            </button>
                        </div>
                    </div>
                    <div class="card-body" id="table-default">
                        <table class="table table-hover table-vcenter card-table w-100" id="data-table">
                            <thead class="h1">
                                <tr>
                                    <th>#</th>
                                    <th>ID</th>
                                    <th>Exact ID</th>
                                    <th>Customer Name</th>
                                    <th>Group</th>
                                    <th>Cash Sale</th>
                                    <th>On Stop</th>
                                    <th>Min. Surcharge</th>
                                    <th>Active</th>
                                    <th></th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>  
        </div>
    </div>    

    <script type="text/javascript">
        let uriMethod = '/Methods/Setting/Customer/CustomerMethod.aspx';
        let roleName = '<%= Session("RoleName") %>';
    </script>
    <script src="/Scripts/Setting/Customer/Default.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

