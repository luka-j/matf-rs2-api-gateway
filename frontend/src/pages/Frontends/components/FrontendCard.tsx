import { FaTrash } from "react-icons/fa";
import { LuRefreshCcw } from "react-icons/lu";

interface IFrontendCardProps {
  name: string;
  url: string;
  endpoints: number;
}

const FrontendCard = ({ name, url, endpoints }: IFrontendCardProps) => {
  return (
    <div className="mt-12 w-full max-w-sm p-4 border rounded-lg shadow sm:p-8 bg-gray-800 border-gray-700">
      <div className="flex items-center justify-between mb-4">
        <LuRefreshCcw className="w-5 h-5 cursor-pointer" />
        <h5 className="text-xl font-bold">{name}</h5>
        <FaTrash className="text-red-600 hover:text-red-700 cursor-pointer" />
      </div>
      <p className="text-lg text-center leading-none">{url}</p>
      <p className="text-center">{endpoints} endpoints</p>
      <div className="flex justify-between items-center mt-4">
        <button className="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm px-4 py-2 text-center mr-3 md:mr-0 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800 text-black">
          View spec
        </button>
        <button className="focus:ring-4 focus:outline-none font-medium rounded-lg text-sm px-4 py-2 text-center mr-3 md:mr-0 bg-blue-600 hover:bg-blue-700 focus:ring-blue-800 text-black">
          View middleware
        </button>
      </div>
    </div>
  );
};

export default FrontendCard;
