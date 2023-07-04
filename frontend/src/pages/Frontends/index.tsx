import { AiFillPlusCircle } from "react-icons/ai";

import FrontendCard from "./components/FrontendCard";

const frontends = [
  {
    id: 1,
    name: "API 1, v1",
    url: "address.com/api/v1",
    endpoints: 10,
  },
  {
    id: 2,
    name: "API 1, v2",
    url: "address.com/api/v2",
    endpoints: 7,
  },
  {
    id: 3,
    name: "API 2, v1",
    url: "address.com/api2/v1",
    endpoints: 5,
  },
  {
    id: 4,
    name: "API 2, v2",
    url: "address.com/api2/v2",
    endpoints: 3,
  },
  {
    id: 5,
    name: "API 3, v1",
    url: "address.com/api3/v1",
    endpoints: 2,
  },
];

const Frontends = () => {
  return (
    <main className="mt-8 mx-auto max-w-7xl pb-8">
      <h1 className="text-4xl text-center font-bold items-center justify-between inline-flex gap-4 w-full">
        Frontends
        <div className="mt-1 hover:bg-gray-600 rounded-full p-1 cursor-pointer transition-all duration-300 active:bg-transparent">
          <AiFillPlusCircle />
        </div>
      </h1>

      <div className="mt-8 block w-full p-6 pt-0  border  rounded-lg shadow  bg-gray-800 border-gray-700">
        <div className="flex justify-between items-center flex-col md:flex-row flex-wrap">
          {frontends.map((frontend) => (
            <FrontendCard key={frontend.id} {...frontend} />
          ))}
        </div>
      </div>
    </main>
  );
};

export default Frontends;
