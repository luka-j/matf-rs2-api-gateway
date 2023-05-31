/* eslint-disable jsx-a11y/anchor-is-valid */
import React from "react";

import { useUserStore } from "@/stores/userStore";
import { cn } from "@/utils/styleUtils";

const Navbar = () => {
  const { logoutUser } = useUserStore((state) => state);
  const [showMobileMenu, setShowMobileMenu] = React.useState(false);

  return (
    <nav>
      <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto p-4">
        <a href="/dashboard" className="flex items-center">
          <span className="self-center text-2xl font-semibold whitespace-nowrap text-white">
            API Gateway
          </span>
        </a>
        <div className="flex md:order-2">
          <button
            onClick={logoutUser}
            className="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm px-4 py-2 text-center mr-3 md:mr-0 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800"
          >
            Logout
          </button>
          <button
            type="button"
            className="inline-flex items-center p-2 text-sm  rounded-lg md:hidden  focus:outline-none focus:ring-2  text-gray-400 hover:bg-gray-700 focus:ring-gray-600"
            aria-controls="navbar-cta"
            aria-expanded="false"
            onClick={() => setShowMobileMenu(!showMobileMenu)}
          >
            <span className="sr-only">Open main menu</span>
            <svg
              className="w-6 h-6"
              aria-hidden="true"
              fill="currentColor"
              viewBox="0 0 20 20"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                fillRule="evenodd"
                d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
                clipRule="evenodd"
              ></path>
            </svg>
          </button>
        </div>
        <div
          className={cn(
            "items-center justify-between  w-full md:flex md:w-auto md:order-1",
            !showMobileMenu && "hidden",
          )}
          id="navbar-cta"
        >
          <ul className="flex flex-col font-medium p-4 md:p-0 mt-4 border  rounded-lg  md:flex-row md:space-x-8 md:mt-0 md:border-0  bg-gray-800 md:bg-gray-900 border-gray-700">
            <li>
              <a
                href="#"
                className="block py-2 pl-3 pr-4 text-white bg-blue-700 rounded md:bg-transparent  md:p-0 md:text-blue-500"
                aria-current="page"
              >
                Example1
              </a>
            </li>
            <li>
              <a
                href="#"
                className="block py-2 pl-3 pr-4  rounded  md:p-0 md:hover:text-blue-500 text-white hover:bg-gray-700 hover:text-white md:hover:bg-transparent border-gray-700"
              >
                Example2
              </a>
            </li>
            <li>
              <a
                href="#"
                className="block py-2 pl-3 pr-4  rounded  md:p-0 md:hover:text-blue-500 text-white hover:bg-gray-700 hover:text-white md:hover:bg-transparent border-gray-700"
              >
                Example3
              </a>
            </li>
            <li>
              <a
                href="#"
                className="block py-2 pl-3 pr-4  rounded  md:p-0 md:hover:text-blue-500 text-white hover:bg-gray-700 hover:text-white md:hover:bg-transparent border-gray-700"
              >
                Example4
              </a>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
