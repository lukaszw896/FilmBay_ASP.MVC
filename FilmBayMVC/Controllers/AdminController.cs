﻿using FilmBayMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FilmBayMVC.Connectivity;
using Microsoft.AspNet.Identity;
using FilmBayMVC.ViewModels;
namespace FilmBayMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            bool isAdmin = DBAccess.isAdmin(User.Identity.GetUserName());
            if (!isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        public ActionResult AdminPanel()
        {

            bool isAdmin = DBAccess.isAdmin(User.Identity.GetUserName());
            if (!isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
       
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult SearchFilmResult(String filmName)
        {
            var list = new List<MovieSearchReturnObjectViewModel>();
            if (filmName != null)
            {


                List<MovieSearchReturnObject> tmpList = null;

                tmpList = TMDbApi.movieSearch(filmName);
                /*
                 * 
                 * DAŁEM SORTOWANIE POPULARNOŚCI ZA POMOCĄ BUBBLE SORTA. TRZEBA ZMIENIĆ!!!!!
                 * 
                 * */
                MovieSearchReturnObject tmp;
                for (int i = 0; i < tmpList.Count(); i++)
                {
                    for (int j = 0; j < tmpList.Count() - 1; j++)
                    {
                        if (tmpList[j].popularity < tmpList[j + 1].popularity)
                        {
                            tmp = tmpList[j];
                            tmpList[j] = tmpList[j + 1];
                            tmpList[j + 1] = tmp;
                        }
                    }

                }
                for (int i = 0; i < tmpList.Count(); i++)
                {
                    MovieSearchReturnObjectViewModel tmpModel = new MovieSearchReturnObjectViewModel();
                    tmpModel.id = tmpList[i].id;
                    tmpModel.orginalTitle = tmpList[i].orginalTitle;
                    tmpModel.popularity = tmpList[i].popularity;
                    tmpModel.posterPath = tmpList[i].posterPath;
                    tmpModel.releaseDate = tmpList[i].releaseDate;
                    tmpModel.title = tmpList[i].title;

                    list.Add(tmpModel);
                }
                ModelsKeeper msaevm = new ModelsKeeper() { movieSearchReturnObjectViewModels = list };

                return PartialView("_SearchResultPartial",msaevm);
            }
            else
                return PartialView("_SearchResultPartial"); 
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public async Task<JavaScriptResult> AddFilmToDatabase(String id,String title,String orginalTitle,String popularity,String releaseDate,String posterPath)
        {
            MovieSearchReturnObject sample = new MovieSearchReturnObject();
            sample.id = int.Parse(id); ;
            sample.orginalTitle = orginalTitle;
            sample.popularity = float.Parse(popularity);
            sample.posterPath = posterPath;
            sample.releaseDate = releaseDate;
            sample.title = title;

            await AddFilmInfo.FilmCreation(sample);
            

            return JavaScript(@"informationPopup(""Dodawanie zakończone (jescze nie sprawdzam czy zakończono pomyślnie :(( ))"")");
        }
        public async Task <ActionResult> EditFilm()
        {
            int id;
            id=1;
            FilmBayMVC.Models.film_table film = new FilmBayMVC.Models.film_table();
            film = await DBAccess.GetFilmById(id);
            ModelsKeeper movieSearchAndEditVM = new ModelsKeeper() { filmTable = film };
            return View(movieSearchAndEditVM);
        }
    }

}