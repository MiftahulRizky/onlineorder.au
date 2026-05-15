<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="Default.aspx.vb" Inherits="_Default" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <div class="container-xl">
                <div class="row g-2 align-items-center">
                    <div class="col">
                        <h2 class="page-title" runat="server" id="pageTitle"></h2>
                    </div>
                </div>
            </div>
        </div>

        <div class="page-body">
            <div class="container-xl">
                <div class="row">
                    <div class="col-12">
                        <div class="card" runat="server" id="divNewsletter">
                            <div class="card-header">
                                <h3 class="card-title">Latest Update & News</h3>
                            </div>

                            <div class="card-body">

                                <div id="carouselExampleFade" class="carousel slide carousel-fade">
                                    <div class="carousel-inner">
                                        <div class="carousel-item active">
                                        <img src="/Content/static/homebanner.png" class="d-block w-100" alt="...">
                                        </div>
                                        <!-- <div class="carousel-item">
                                        <img src="..." class="d-block w-100" alt="...">
                                        </div>
                                        <div class="carousel-item">
                                        <img src="..." class="d-block w-100" alt="...">
                                        </div> -->
                                    </div>
                                    <!-- <button class="carousel-control-prev" type="button" data-bs-target="#carouselExampleFade" data-bs-slide="prev">
                                        <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                                        <span class="visually-hidden">Previous</span>
                                    </button>
                                    <button class="carousel-control-next" type="button" data-bs-target="#carouselExampleFade" data-bs-slide="next">
                                        <span class="carousel-control-next-icon" aria-hidden="true"></span>
                                        <span class="visually-hidden">Next</span>
                                    </button> -->
                                </div>
                                
                            </div>

                            <div class="card-footer text-end"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <script type="text/javascript">
            document.addEventListener("DOMContentLoaded", () => {
                loaderFadeOut();
            })

         
        </script>
    </asp:Content>